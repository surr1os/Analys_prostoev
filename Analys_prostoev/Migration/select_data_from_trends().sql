-- FUNCTION: public.select_data_from_trends()

-- DROP FUNCTION IF EXISTS public.select_data_from_trends();

CREATE OR REPLACE FUNCTION public.select_data_from_trends()
    RETURNS void
    LANGUAGE plpgsql
AS $BODY$
DECLARE
    matched_row             record;
    analysis_tests          record;
    row_id                  integer;
    row_t                   timestamp;
    row_v                   integer;
    calculated_period       integer;
    date_finish_for_insert  timestamp;
    is_present_end          boolean;
    trends_id               integer;
    shift_letter            varchar(1);
BEGIN
    CREATE TEMP TABLE temp_analysis_tests 
    (
        id              integer,
        date_start      timestamp NOT NULL,
        date_finish     timestamp,
        region          text,
        period          integer,
        shift_letter    varchar(1) -- Добавляем колонку для буквы смены
    );

    <<main_loop>>
    FOR row_id, row_t, row_v IN (
        SELECT id, t, v 
        FROM public.trends
        WHERE t::date BETWEEN current_date - interval '2 day' AND current_date
            AND l = 0 AND v = 0 
        ORDER BY t
    )
    LOOP
        is_present_end          := false;
        date_finish_for_insert  := row_t; 
        calculated_period       := 0;

        <<scnd_loop>>
        FOR matched_row IN (
            SELECT * 
            FROM public.trends
            WHERE t >= row_t
                AND l = 0 AND id = row_id  
            ORDER BY t
        )
        LOOP
            IF EXISTS(
                SELECT 1
                FROM temp_analysis_tests
                WHERE date_finish >= matched_row.t
                    AND id = row_id
            )
            THEN
                EXIT;
            END IF;
              
            IF is_present_end = true AND matched_row.v = 0
            THEN
                EXIT;
            END IF;
              
            IF is_present_end = false AND matched_row.v = 0
            THEN
                CONTINUE scnd_loop;
            END IF;
              
            IF NOT EXISTS(
                SELECT 1
                FROM temp_analysis_tests
                WHERE date_finish = matched_row.t
                    AND id = row_id
            )
            THEN
                IF NOT EXISTS(
                    SELECT 1
                    FROM temp_analysis_tests
                    WHERE date_start = matched_row.t
                        AND id = row_id
                )
                THEN
                    calculated_period       := extract(EPOCH FROM (matched_row.t - row_t)) / 60;
                    date_finish_for_insert  := matched_row.t;
                    trends_id               := matched_row.id;
                    is_present_end          := true;

                    -- Определение буквы смены
                    SELECT s.letter
                    INTO shift_letter
                    FROM public.shifts s
                    JOIN public.time_shifts ts ON s.time_shift_id = ts.id
                    WHERE s.day = matched_row.t::date
                        AND matched_row.t::time BETWEEN ts.time_begin AND ts.time_end;

                    IF shift_letter IS NULL
                    THEN
                        shift_letter := NULL;
                    END IF;
                END IF;
            END IF;
        END LOOP;

        IF is_present_end = true
        THEN
            INSERT INTO temp_analysis_tests (id, date_start, date_finish, region, period, shift_letter)
            VALUES (row_id, row_t, date_finish_for_insert, (SELECT description FROM public.variables WHERE id = trends_id), calculated_period, shift_letter);
        END IF;
    END LOOP;

    FOR analysis_tests IN SELECT * FROM temp_analysis_tests WHERE region IS NOT NULL
    LOOP 
        IF analysis_tests.date_finish IS NOT NULL
        THEN
            IF NOT EXISTS(
                SELECT 1
                FROM public.analysis
                WHERE date_start = analysis_tests.date_start
                    AND region = analysis_tests.region
            )
            THEN
                INSERT INTO public.analysis (date_start, date_finish, region, period, shifts)
                VALUES (analysis_tests.date_start, 
                        analysis_tests.date_finish, 
                        analysis_tests.region,
                        analysis_tests.period,
                        analysis_tests.shift_letter); -- Добавляем значение буквы смены в колонку shifts
            ELSE
                UPDATE public.analysis
                SET date_finish = analysis_tests.date_finish,
                    shifts = analysis_tests.shift_letter -- Обновляем значение буквы смены
                WHERE date_start = analysis_tests.date_start
                    AND date_finish < analysis_tests.date_finish
                    AND region = analysis_tests.region;
            END IF;
        END IF;
    END LOOP;

    DROP TABLE IF EXISTS temp_analysis_tests;
END;
$BODY$;
