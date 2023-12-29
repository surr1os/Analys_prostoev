Alter table public.analysis 
drop column if exists created_at;
Alter table public.analysis
add column created_at timestamp not null default date_start;

Alter table public.analysis 
drop column  if exists change_at;
Alter table public.analysis
add column change_at timestamp not null default date_finish;

Alter table public.analysis 
drop column if exists is_manual;
Alter table public.analysis
add column is_manual bool not null default false;

ALTER TABLE public.analysis
ALTER COLUMN status TYPE TEXT;

alter table public.analysis
add column if not exists shifts text;
