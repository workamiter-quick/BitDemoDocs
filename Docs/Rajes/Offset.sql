select * from [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES] where Development_footprint_ID != ''

select * from [biofin].[dbo].[Offset]


select distinct o.id, 
r.OffsetUniqueId,
r.applicationtitle as Project_Title,
da.[Name] as Applicant,
r.id as Biodiversity_offset_site_ID,case when au.permitdate is null  then  '' else YEAR(au.permitdate) end as Year_of_authorisation,
ot.[Name] as Offset_Type,
mfi.[Name] as Offset_Implementation_Progress 
--into _TempOffset
from registrationdetail r 
left join  RegistrationDetailandDevelopmentApplicantMapping a on r.id = a.RegistrationDetailId 
left join MstDevelopmentApplicant da on a. MstDevelopmentApplicantId = da.id
left join Authorisation au on r.id = au.RegistrationDetailsId 
left join offset o on au.id = o.AuthorisationId 
left join MstOffsetType ot on ot.id = o.MstOffsetTypeId 
left Join OffsetImplementation oi on oi.OffsetId = o.id
left join MstOffsetImplementation mfi on mfi.id = oi.MstOffsetImplementationId


where r.OffsetUniqueId in (select distinct Development_footprint_ID from [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES] where Development_footprint_ID != '')


need to update uniqueID with r.id
Unique_ID from [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES]

write query for me


UPDATE bos
SET bos.Unique_ID = r.id

select distinct r.OffsetUniqueId, r.id
into __TempOffset2
FROM [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES] bos
INNER JOIN _TempOffset r 
    ON bos.Development_footprint_ID = r.OffsetUniqueId




WHERE bos.Development_footprint_ID = 'GPGP07-001'

OffsetUniqueId = 'GPGP07-001'


select * from _TempOffset

UPDATE [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES]
SET Unique_ID = r.id

--select distinct r.OffsetUniqueId, r.id
FROM [biofinspatial].[dbo].[BIODIVERSITY_OFFSET_SITES] dp
INNER JOIN _TempOffset r 
    ON dp.Development_footprint_ID = r.OffsetUniqueId


