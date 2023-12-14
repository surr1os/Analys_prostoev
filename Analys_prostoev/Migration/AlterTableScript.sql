Alter table public.analysis 
drop column if exists created_at;
Alter table public.analysis
add column created_at timestamp not null default current_timestamp;

Alter table public.analysis 
drop column  if exists change_at;
Alter table public.analysis
add column change_at timestamp not null default current_timestamp;

Alter table public.analysis 
drop column if exists is_manual;
Alter table public.analysis
add column is_manual bool not null default false;

update public.analysis set created_at = date_start 

update public.analysis set change_at = date_finish 
