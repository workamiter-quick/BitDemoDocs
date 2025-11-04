select distinct 
r.id, 
r.OffsetUniqueId,
r.applicationtitle as Project_Title,
da.[Name] as Applicant, 
da.id as  development_footprint_ID,
case when au.permitdate is null  then  '' else YEAR(au.permitdate) end as Year_of_authorisation,
ot.[Name] as Offset_Type,
mfi.[Name] as Offset_Implementation_Progress 
from registrationdetail r 
left join  RegistrationDetailandDevelopmentApplicantMapping a on r.id = a.RegistrationDetailId 
left join  RegistrationDetailandDevelopmentTypeMapping dy 
on r.id = dy.RegistrationDetailId 
left join MstDevelopmentApplicant da on a. MstDevelopmentApplicantId = da.id 
left join Authorisation au on r.id = au.RegistrationDetailsId 
left join offset o on au.id = o.AuthorisationId 
left join MstOffsetType ot on ot.id = o.MstOffsetTypeId 
left Join OffsetImplementation oi on oi.OffsetId = o.id 
left join MstOffsetImplementation mfi on mfi.id = oi.MstOffsetImplementationId
where r.OffsetUniqueId in (select distinct Development_footprint_ID from [biofinspatial].[dbo].[DEVELOPMENT_PROJECT])

select * from [biofinspatial].[dbo].[DEVELOPMENT_PROJECT]



need to update uniqueID with r.id
uniqueID from [biofinspatial].[dbo].[DEVELOPMENT_PROJECT]

write query for me


UPDATE [biofinspatial].[dbo].[DEVELOPMENT_PROJECT]
SET Unique_ID = r.id

--select distinct r.OffsetUniqueId, r.id
FROM [biofinspatial].[dbo].[DEVELOPMENT_PROJECT] dp
INNER JOIN registrationdetail r 
    ON dp.Development_footprint_ID = r.OffsetUniqueId
	where r.OffsetUniqueId = 'GPGP07-001'

	select count(*) from registrationdetail

