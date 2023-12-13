Alter table public.analysis 
drop column is_manual
alter column created_at not null default current_timestamp

Alter table public.analysis 
drop column change_at
alter column change_at not null default current_timestamp

Alter table public.analysis 
drop column is_manual
add column is_manual bool not null default false;

update public.analysis set created_at = date_start 

update public.analysis set change_at = date_finish 
