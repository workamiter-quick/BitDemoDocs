select top 5 * from [dbo].[BIODIVERSITY_OFFSET_SITES]


select distinct r.id, r.applicationtitle as Project_Title,da.[Name] as Applicant, da.id as  development_footprint_ID, case when au.permitdate is null  
then  '' else YEAR(au.permitdate) end as Year_of_authorisation,ot.[Name] as Offset_Type,mfi.[Name] as Offset_Implementation_Progress from registrationdetail r left join
RegistrationDetailandDevelopmentApplicantMapping a on r.id = a.RegistrationDetailId left join  RegistrationDetailandDevelopmentTypeMapping dy on r.id = dy.RegistrationDetailId 
left join MstDevelopmentApplicant da on a. MstDevelopmentApplicantId = da.id left join Authorisation au on r.id = au.RegistrationDetailsId left join offset o on au.id = o.AuthorisationId 
left join MstOffsetType ot on ot.id = o.MstOffsetTypeId left Join OffsetImplementation oi on oi.OffsetId = o.id left join MstOffsetImplementation mfi on mfi.id = oi.MstOffsetImplementationId


select * from [dbo].[BIODIVERSITY_OFFSET_SITES] where Unique_ID = 'DA24-00067'

update BIODIVERSITY_OFFSET_SITES set Development_footprint_ID = Unique_ID

ALTER TABLE BIODIVERSITY_OFFSET_SITES CHANGE Biodiversity_offset Development_footprint_ID nvarchar(50);

EXEC sp_rename 'BIODIVERSITY_OFFSET_SITES.Biodiversity_offset', 'Development_footprint_ID', 'COLUMN';

SELECT DISTINCT PROVINCE FROM MUNICIPALITIES



select * from [dbo].[DEVELOPMENT_PROJECT]