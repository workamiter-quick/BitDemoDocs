-- Table: public.bookmark

-- DROP TABLE IF EXISTS public.bookmark;

CREATE TABLE IF NOT EXISTS public.bookmark
(
    brk_id integer NOT NULL DEFAULT nextval('bookmark_brk_id_seq'::regclass),
    caption character varying(250) COLLATE pg_catalog."default",
    url character varying(2000) COLLATE pg_catalog."default",
    userid integer DEFAULT 0,
    isactive character varying(20) COLLATE pg_catalog."default" DEFAULT 0,
    CONSTRAINT bookmark_pkey PRIMARY KEY (brk_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.bookmark
    OWNER to postgres;