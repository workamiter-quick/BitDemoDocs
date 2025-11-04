-- Table: public.wms_config

-- DROP TABLE IF EXISTS public.wms_config;

CREATE TABLE IF NOT EXISTS public.wms_config
(
    objectid integer NOT NULL DEFAULT nextval('wms_config_objectid_seq'::regclass),
    layer_alias_name character varying(500) COLLATE pg_catalog."default",
    layer_wms_url character varying(500) COLLATE pg_catalog."default",
    layer_name character varying(500) COLLATE pg_catalog."default",
    fomart character varying(200) COLLATE pg_catalog."default",
    transparent character varying(200) COLLATE pg_catalog."default",
    tiled character varying(200) COLLATE pg_catalog."default",
    buffer character varying(200) COLLATE pg_catalog."default",
    display_outside_max_extent character varying(200) COLLATE pg_catalog."default",
    baselayer character varying(200) COLLATE pg_catalog."default",
    display_in_layerswitcher character varying(200) COLLATE pg_catalog."default",
    visibility character varying(200) COLLATE pg_catalog."default",
    isdeleted integer,
    groupid integer,
    userid character varying(100) COLLATE pg_catalog."default",
    layer_index integer,
    CONSTRAINT wms_config_pkey PRIMARY KEY (objectid)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.wms_config
    OWNER to postgres;