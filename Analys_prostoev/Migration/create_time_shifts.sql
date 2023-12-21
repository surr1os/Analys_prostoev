CREATE TABLE IF NOT EXISTS public.time_shifts
(
 id SERIAL PRIMARY KEY,
 time_begin TIME,
 time_end TIME
);


insert into public.time_shifts (time_begin, time_end) values ('08:00:00', '20:00:00');
insert into public.time_shifts (time_begin, time_end) values ('20:00:00', '08:00:00');

select * from public.time_shifts