-- DROP FUNCTION public.analysis_change_status();

CREATE OR REPLACE FUNCTION public.analysis_change_status()
 RETURNS void
 LANGUAGE sql
AS $function$
--begin 
	update public.analysis set status = 1 
	where (status is Null or status = 0) and is_manual = false and change_at <= (current_timestamp - interval '1 day');
--end;
$function$;