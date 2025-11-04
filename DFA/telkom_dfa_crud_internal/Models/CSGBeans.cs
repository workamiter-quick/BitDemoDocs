using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crud.Models
{
    public class CSGBeans
    {
    }

    public class LayerGroup
    {

    }

    public class Layers
    {

    }


    public class SearchInput
    {
        public string ProvinceCode { get; set; }
        public string SearchType { get; set; }
        public string DMCode { get; set; }
        public string LMCode { get; set; }
        public string ARCode { get; set; }
        public string SearchCriteria { get; set; }
        public string FarmName { get; set; }
        public string TownByName { get; set; }
        public string SGNumber { get; set; }
        public string LPI { get; set; }
        public string Region { get; set; }
        public string Parcel { get; set; }
        public string Portion { get; set; }
    }


    public class LPIS_ARR
    {
        public List<string> LPIs { get; set; }
    }

    public class LOCATION_ARR
    {
        public string LocationID1 { get; set; }
        public string LocationID2 { get; set; }
        public string BufferValue { get; set; }
        public string LPIs { get; set; }
        public string Actions { get; set; }
    }

    public class IdentifyInput
    {
        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public string[] lstVisibleLayer { get; set; }
    }

    public class ChildParentCordData
    {
        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public string LOProj { get; set; }
    }

    public class ChildParentData
    {
        public int RecordID { get; set; }
        public string RecordName { get; set; }//P = parent, C = Chilld

        public List<ChildParentCordData> lstChildParentCordData = new List<ChildParentCordData>();
    }

    public class ApiKeyInput
    {
        public string CompanyName { get; set; }
        public string RequestorName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class BookmarkInput
    {
        public string Caption { get; set; }
        public string URL { get; set; }
        public string UserID { get; set; }
    }

    public class VDBeans
    {
        public string VDData { get; set; }
        public string SelectedWardID { get; set; }
    }

    public class GoogleCoord
    {
        public string Longitude_X { get; set; }
        public string Latitude_Y { get; set; }
        public string Bandwidth { get; set; }
        public string ContractTerm { get; set; }
        public string Longitude_X_Green { get; set; }
        public string Latitude_Y_Green { get; set; }
        public string DDMaxDistance { get; set; }
        public string UseMaxDistance { get; set; }
    }

    public class SiteDetails
    {
        [JsonProperty("GID")]
        public string GID { get; set; }

        [JsonProperty("Product")]
        public string Product { get; set; } = "--";

        [JsonProperty("Latitude_Y")]
        public string Latitude_Y { get; set; } = "--";

        [JsonProperty("Longitude_X")]
        public string Longitude_X { get; set; } = "--";

        [JsonProperty("PoP")]
        public string PoP { get; set; } = "--";

        [JsonProperty("Distance_Meters")]
        public string Distance_Meters { get; set; } = "--";

        [JsonProperty("Distance_Meters_Int")]
        public int Distance_Meters_Int { get; set; } = 0;

        [JsonProperty("MRC")]
        public string MRC { get; set; } = "--";

        [JsonProperty("NRC")]
        public string NRC { get; set; } = "--";

        [JsonProperty("PolygonCount")]
        public string PolygonCount { get; set; } = "--";

        [JsonProperty("Operator")]
        public string Operator { get; set; } = "--";

        [JsonProperty("Network_Type")]
        public string Network_Type { get; set; } = "--";

        [JsonProperty("Wayleave_NRC")]
        public string Wayleave_NRC { get; set; } = "--";        

        [JsonProperty("Special_Build_NRC")]
        public string Special_Build_NRC { get; set; } = "--";

        [JsonProperty("Party_Provider_3rd")]
        public string Party_Provider_3rd { get; set; } = "--";

        [JsonProperty("Party_MRC_3rd")]
        public string Party_MRC_3rd { get; set; } = "--";

        [JsonProperty("Party_NRC_3rd")]
        public string Party_NRC_3rd { get; set; } = "--";

        [JsonProperty("Total_MRC")]
        public string Total_MRC { get; set; } = "--";

        [JsonProperty("Total_NRC")]
        public string Total_NRC { get; set; } = "--";

        [JsonProperty("Notes")]
        public string Notes { get; set; } = "--";

        [JsonProperty("IsCPTPTA")]
        public bool IsCPTPTA { get; set; } = false;

        [JsonProperty("ChkUseMaxDistance")]
        public bool chkUseMaxDistance { get; set; } = false;

        [JsonProperty("ChkDDMaxDistance")]
        public bool chkDDMaxDistance { get; set; } = false;











    }

    public class SiteDetailsApi
    {
        [JsonProperty("GID")]
        public string GID { get; set; }

        [JsonProperty("Product")]
        public string Product { get; set; } = "--";

        [JsonProperty("Latitude_Y")]
        public string Latitude_Y { get; set; } = "--";

        [JsonProperty("Longitude_X")]
        public string Longitude_X { get; set; } = "--";

        [JsonProperty("PoP")]
        public string PoP { get; set; } = "--";

        [JsonProperty("Distance_Meters")]
        public string Distance_Meters { get; set; } = "--";

        [JsonProperty("Distance_Meters_Int")]
        public int Distance_Meters_Int { get; set; } = 0;

        [JsonProperty("MRC")]
        public string MRC { get; set; } = "--";

        [JsonProperty("NRC")]
        public string NRC { get; set; } = "--";

        [JsonProperty("PolygonCount")]
        public string PolygonCount { get; set; } = "--";

        [JsonProperty("Operator")]
        public string Operator { get; set; } = "--";

        [JsonProperty("Network_Type")]
        public string Network_Type { get; set; } = "--";

        [JsonProperty("Wayleave_NRC")]
        public string Wayleave_NRC { get; set; } = "--";

        [JsonProperty("Special_Build_NRC")]
        public string Special_Build_NRC { get; set; } = "--";

        [JsonProperty("Party_Provider_3rd")]
        public string Party_Provider_3rd { get; set; } = "--";

        [JsonProperty("Party_MRC_3rd")]
        public string Party_MRC_3rd { get; set; } = "--";

        [JsonProperty("Party_NRC_3rd")]
        public string Party_NRC_3rd { get; set; } = "--";

        [JsonProperty("Total_MRC")]
        public string Total_MRC { get; set; } = "--";

        [JsonProperty("Total_NRC")]
        public string Total_NRC { get; set; } = "--";

        [JsonProperty("Notes")]
        public string Notes { get; set; } = "--";

        [JsonProperty("Term")]
        public string Term { get; set; } = "--";

        [JsonProperty("Bandwidth")]
        public string Bandwidth { get; set; } = "--";

        [JsonProperty("IsCPTPTA")]
        public bool IsCPTPTA { get; set; } = false;

        [JsonProperty("ChkUseMaxDistance")]
        public bool chkUseMaxDistance { get; set; } = false;

        [JsonProperty("ChkDDMaxDistance")]
        public bool chkDDMaxDistance { get; set; } = false;











    }
    public class SplitedData
    {
        public string DataOutput { get; set; }
    }


    public class VDChildBeans
    {
        public int VD_Number { get; set; }
        public int RegPop { get; set; }
    }

    public class GroupBeans
    {
        public string GroupName { get; set; }
        public string UserID { get; set; }
    }

    public class WmsBeans
    {
        public string objectid { get; set; }
        public string layer_alias_name { get; set; }
        public string layer_wms_url { get; set; }
        public string layer_name { get; set; }
        public string fomart { get; set; }
        public string transparent { get; set; }
        public string tiled { get; set; }
        public string buffer { get; set; }
        public string display_outside_max_extent { get; set; }
        public string baselayer { get; set; }
        public string display_in_layerswitcher { get; set; }
        public string visibility { get; set; }
        public string isdeleted { get; set; }
        public string groupid { get; set; }
        public string userid { get; set; }
        public string layer_index { get; set; }
    }




    public class StreetLineCenterInput
    {
        public string XCoord { get; set; }
        public string YCoord { get; set; }
    }

    public class AnyIDs
    {
        public List<string> IDS { get; set; }
    }


    public class AddressSearch
    {
        public string Address { get; set; }
    }

    public class RoutInputCoord
    {
        public string StartLat_Y { get; set; }
        public string StartLng_X { get; set; }
        public string EndLat_Y { get; set; }
        public string EndLng_X { get; set; }
        public string Route_Mode { get; set; }
    }

    public class ApiAccessRequestDto
    {
        public string CompanyName { get; set; }
        public string RequestorName { get; set; }
        public string EmailAddress { get; set; }
        public string ApiKey { get; set; }
    }


    public class RootObject
    {
        [JsonProperty("prop")]
        public Prop Prop { get; set; }

        [JsonProperty("splitedwkt")]
        public List<string> Splitedwkt { get; set; }

        [JsonProperty("regPopValues")]
        public RegPopValues RegPopValues { get; set; }

        [JsonProperty("polygonWardNumber")]
        public PolygonWardNumber PolygonWardNumber { get; set; }
    }

    public class Prop
    {
        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("cat_b")]
        public string CatB { get; set; }

        [JsonProperty("regpop")]
        public int Regpop { get; set; }

        [JsonProperty("vdnumber")]
        public long Vdnumber { get; set; }

        // Add other properties as needed
    }

    public class RegPopValues
    {
        [JsonProperty("val_A")]
        public int ValA { get; set; }

        [JsonProperty("val_B")]
        public int ValB { get; set; }
    }

    public class PolygonWardNumber
    {
        [JsonProperty("WardNum_A")]
        public int WardNumA { get; set; }

        [JsonProperty("WardNum_B")]
        public int WardNumB { get; set; }
    }

    public class BackupVDDeatils
    {

        [JsonProperty("ID")]
        public int id { get; set; }

        [JsonProperty("VdNumber")]
        public int VdNumber { get; set; }

        [JsonProperty("Wkt")]
        public string WKT { get; set; }

    }

}
