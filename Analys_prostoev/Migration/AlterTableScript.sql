Alter table public.analysis 
drop column if exists created_at;
Alter table public.analysis
add column created_at timestamp not null default date_start;

Alter table public.analysis 
drop column  if exists change_at;
Alter table public.analysis
add column change_at timestamp null default date_start;

Alter table public.analysis 
drop column if exists is_manual;
Alter table public.analysis
add column is_manual bool not null default false;

update public.analysis set created_at = date_start 

update public.analysis set change_at = date_start  

ALTER TABLE public.analysis
ALTER COLUMN status TYPE TEXT;

alter table public.analysis
add column if not exists shifts text;

CREATE TABLE is not exists public.change_history (
	"Id" int8 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE),
	region text NULL,
	date_change timestamp NULL,
	id_pros int8 NULL,
	modified_element text NULL,
	CONSTRAINT change_history_pkey PRIMARY KEY ("Id")
);

