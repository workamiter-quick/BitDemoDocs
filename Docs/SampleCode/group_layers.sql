-- Table: public.group_layers

-- DROP TABLE IF EXISTS public.group_layers;

CREATE TABLE IF NOT EXISTS public.group_layers
(
    objectid integer NOT NULL DEFAULT nextval('group_layers_objectid_seq'::regclass),
    group_name character varying(500) COLLATE pg_catalog."default",
    isdeleted integer,
    userid character varying(20) COLLATE pg_catalog."default",
    CONSTRAINT group_layers_pkey PRIMARY KEY (objectid)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.group_layers
    OWNER to postgres;