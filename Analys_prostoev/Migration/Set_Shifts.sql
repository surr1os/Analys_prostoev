UPDATE analysis 
SET shifts = shifts_table.letter
FROM (
    SELECT a."Id", s.letter
    FROM analysis a
    INNER JOIN shifts s ON a.date_start::date = s."day"
    INNER JOIN (
        SELECT 
            CASE
                WHEN ts."time_end" > ts."time_begin" THEN
                    (a.date_start::time >= ts."time_begin" AND a.date_start::time <= ts."time_end")
                ELSE
                    (a.date_start::time >= ts."time_begin" OR a.date_start::time <= ts."time_end")
            END AS is_shift,
            a."Id",
            s.time_shift_id,
            ROW_NUMBER() OVER (PARTITION BY a."Id" ORDER BY ts."time_begin" DESC) AS rn
        FROM analysis a
        INNER JOIN shifts s ON a.date_start::date = s."day"
        INNER JOIN time_shifts ts ON ts.id = s.time_shift_id
    ) sub ON sub."Id" = a."Id" AND sub.time_shift_id = s.time_shift_id
    WHERE is_shift
) shifts_table
WHERE analysis."Id" = shifts_table."Id";
