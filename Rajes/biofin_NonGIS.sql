select distinct r.id, r.OffsetUniqueId, r.applicationtitle as Project_Title,da.[Name] as Applicant, da.id as  development_footprint_ID, case when au.permitdate is null  
then  '' else YEAR(au.permitdate) end as Year_of_authorisation,ot.[Name] as Offset_Type,mfi.[Name] as Offset_Implementation_Progress from registrationdetail r left join
RegistrationDetailandDevelopmentApplicantMapping a on r.id = a.RegistrationDetailId left join  RegistrationDetailandDevelopmentTypeMapping dy on r.id = dy.RegistrationDetailId 
left join MstDevelopmentApplicant da on a. MstDevelopmentApplicantId = da.id left join Authorisation au on r.id = au.RegistrationDetailsId left join offset o on au.id = o.AuthorisationId 
left join MstOffsetType ot on ot.id = o.MstOffsetTypeId left Join OffsetImplementation oi on oi.OffsetId = o.id left join MstOffsetImplementation mfi on mfi.id = oi.MstOffsetImplementationId
where r.id = 253

select id, MunicipalArea, SGCode, OffsetUniqueId, ActivityLocationFarm, ActivityLocationPortion from registrationdetail where ID = 253
select RegistrationDetailId,MstLocationId,CreatedBy from RegistrationDetailandDevelopmentProvinceMapping where RegistrationDetailId = 253

delete from RegistrationDetailandDevelopmentProvinceMapping where RegistrationDetailId = 248

insert into RegistrationDetailandDevelopmentProvinceMapping (RegistrationDetailId,MstLocationId,CreatedBy) Values (248,(select Id from [MstLocation]  where Name = 'Free State'),1 )

select distinct o.id, r.OffsetUniqueId, r.applicationtitle as Project_Title,da.[Name] as Applicant,r.id as Biodiversity_offset_site_ID,case when au.permitdate is null  then  '' else YEAR(au.permitdate) end as Year_of_authorisation,
ot.[Name] as Offset_Type,mfi.[Name] as Offset_Implementation_Progress from registrationdetail r left join  RegistrationDetailandDevelopmentApplicantMapping a on r.id = a.RegistrationDetailId left join MstDevelopmentApplicant da on a. MstDevelopmentApplicantId = da.id
left join Authorisation au on r.id = au.RegistrationDetailsId left join offset o on au.id = o.AuthorisationId left join MstOffsetType ot on ot.id = o.MstOffsetTypeId left Join OffsetImplementation oi on oi.OffsetId = o.id
left join MstOffsetImplementation mfi on mfi.id = oi.MstOffsetImplementationId where o.id = 134 order by 2

select id, SGCode, OffsetLocation, MuncipalArea, OffsetLocationPortion, OffsetLocationPortion from offset where ID = 134



RegistrationDetailandDevelopmentTypeMapping

select OffsetUniqueId, * from registrationdetail

select * from [MstLocation] where Name in (SELECT DISTINCT PROVINCE FROM biofinspatial.[dbo].[MUNICIPALITIES])  --Code = @Province

SELECT DISTINCT PROVINCE FROM biofinspatial.[dbo].[MUNICIPALITIES]