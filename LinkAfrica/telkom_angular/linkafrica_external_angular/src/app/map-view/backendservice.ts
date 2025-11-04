import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { LPIS_ARR, LOCATION_ARR } from './ApplicationsBeans';
import { ConfigManager } from '../config-ang';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LPIService {
  //serviceBaseURL = 'https://localhost:44354/'; // Devlopment
  //serviceBaseURL = 'http://10.1.15.113:8001/'; // Terra Environment

  constructor(private http: HttpClient, private configmanager: ConfigManager) { }
  serviceBaseURL = this.configmanager.serviceBaseURL;
  serviceExternalURL = this.configmanager.serviceExternalURL;
  serviceGeoserverURL = this.configmanager.serviceGeoserverURL;

  getData(data: any) {
    var LpiData: any;
    LpiData = '0';
    data.forEach(function (value: any) {
      LpiData = LpiData + '|' + value;
    });
    let url = this.serviceBaseURL + 'LPIData/' + LpiData;
    return this.http.get(url);
  }

  getLMData() {
    let url = this.serviceBaseURL + 'LM/0';
    return this.http.get(url);
  }
  getProvinceData(PID: any) {
    let url = this.serviceBaseURL + 'Location/' + PID;
    return this.http.get(url);
  }

  getDistrictMunicData(PID: any) {
    let url = this.serviceBaseURL + 'Location/GetDM/' + PID;
    return this.http.get(url);
  }

  getLocalMunicData(DMID: any) {
    let url = this.serviceBaseURL + 'Location/GetLM/' + DMID;
    return this.http.get(url);
  }

  getAdminRegionData(PID: any) {
    let url = this.serviceBaseURL + 'Location/GetAR/' + PID;
    return this.http.get(url);
  }

  getRPPDataBYAdminRegion(ARID: any) {
    let url = this.serviceBaseURL + 'Location/GetRegionByAR/' + ARID;
    return this.http.get(url);
  }

  getRPPDataBYDMMetro(DMMetroID: any) {
    let url = this.serviceBaseURL + 'Location/GetRegionByDMMetro/' + DMMetroID;
    return this.http.get(url);
  }

  getRPPDataBYLM(LMID: any) {
    let url = this.serviceBaseURL + 'Location/GetRegionByLM/' + LMID;
    return this.http.get(url);
  }

  getReservationLPI(PAR: any) {
    let url = this.serviceBaseURL + 'App/GetReservationLPI/' + PAR;
    return this.http.get(url);
  }

  getLodgementLPI(PAR: any) {
    let url = this.serviceBaseURL + 'App/GetLodgementLPI/' + PAR;
    return this.http.get(url);
  }

  getChildParentOverlayData(PAR: any) {
    let url =
      this.serviceBaseURL + 'App/GetChildParentOverlayData/' + PAR + '/Record';
    return this.http.get(url);
  }

  getSearchResult(SI: any) {
    let url = this.serviceBaseURL + 'Search';
    return this.http.post(url, SI);
  }

  getLPIsRealtionship(LPIS: any) {
    let url = this.serviceBaseURL + 'SpatialQuery';
    return this.http.post(url, LPIS);
  }

  getLocationRealtionship(TArr: any) {
    let url = this.serviceBaseURL + 'LocationSpatialQuery';
    return this.http.post(url, TArr);
  }

  getIdentifyResult(IIB: any) {
    let url = this.serviceBaseURL + 'Identify';
    return this.http.post(url, IIB);
  }

  getGoogleAddressResult(address: string): Observable<any> {
    let url = this.serviceBaseURL + 'Geocoding/GetCoordinates';
    return this.http.post(url, address);
  }

  getRouteDataResult(coordinates: any): Observable<any> {
    let url = this.serviceBaseURL + 'Geocoding/GetRoute';
    return this.http.post(url, coordinates);
  }

  getSiteDetailResult(googleCord: any): Observable<any> {
    let url = this.serviceBaseURL + 'VectorSpatial/GetSiteDetail';
    return this.http.post(url, googleCord);
  }

  getSiteDetailGreenResult(googleCord: any): Observable<any> {
    let url = this.serviceBaseURL + 'VectorSpatial/GetSiteDetailGreen';
    return this.http.post(url, googleCord);
  }

  getAddressDataResult(addressStr: any): Observable<any> {    
    //let url = this.serviceBaseURL + 'VectorSpatial/GetAddressData';
    let url = this.serviceBaseURL + 'Geocoding/GetAddressData';
    return this.http.post(url, addressStr);
  }

  getCaptureDataResult(IIB: any) {
    let url = this.serviceBaseURL + 'CaptureData';
    return this.http.post(url, IIB);
  }

  getStreetData(StreetObjectID: any) {
    let url = this.serviceBaseURL + 'Street/' + StreetObjectID;
    return this.http.get(url);
  }

  getBingAddressSearch(AddressData: any) {
    let url =
      'http://dev.virtualearth.net/REST/v1/Locations?query=' +
      AddressData +
      '&key= AnFjYYuyu9GzzkGy-X2XruPkf4Imc8KZEPeXIGAqdkg9dfVmWvXBVit3UR5B3B6r&json=?';
    return this.http.get(url);
  }

  getBookmarkData(UID: any) {
    let url = this.serviceBaseURL + 'Bookmark/' + UID;
    return this.http.get(url);
  }

  saveBookmark(BMI: any) {
    let url = this.serviceBaseURL + 'Bookmark';
    return this.http.post(url, BMI);
  }

  deleteBookmark(BMI: any) {
    let url = this.serviceBaseURL + 'Bookmark/' + BMI;
    return this.http.delete(url);
  }

  getLocationByID(ID: any) {
    let url = this.serviceBaseURL + 'Location/GetLocationByID/' + ID;
    return this.http.get(url);
  }

  getTownByID(ID: any) {
    let url = this.serviceBaseURL + 'Location/GetTownByID/' + ID;
    return this.http.get(url);
  }

  getLocationsByID(IDs: any) {
    let url = this.serviceBaseURL + 'Location/GetLocationsID/' + IDs;
    return this.http.get(url);
  }

  getConfigData() {
    let url = this.serviceBaseURL + 'Config';
    return this.http.get(url);
  }

  TokenGenrator(ExternalToken: any) {
    var data = ExternalToken.split('|');

    const tokenURL = data[2].split('=')[1];

    const loginPayload = new HttpParams()
      .set('grant_type', 'password')
      .set('username', data[0].split('=')[1])
      .set('password', data[1].split('=')[1]);

    //const Authorization_Val = 'Basic ' + window.btoa('cis-rest-api-gis:XY7kmzoNzl100');

    let httpheaders = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded', //If your header name has spaces or any other char not appropriate
      Authorization: 'Basic ' + window.btoa('cis-rest-api-gis:XY7kmzoNzl100'), //for object property name, use quoted notation shown in second
    });

    return this.http.post(tokenURL, loginPayload, { headers: httpheaders });

    //return null;
  }

  pushedCaptureParcel(LPIS: any) {
    var GIS_JWT_TOKEN = localStorage.getItem('GIS_JWT_TOKEN');
    const hdnUAM: any = document.getElementById('hdnUAM')!;
    const hdnStepID: any = document.getElementById('hdnStepID')!;

    let httpheaders = new HttpHeaders({
      'Content-Type': 'application/json', //If your header name has spaces or any other char not appropriate
      Authorization: 'bearer ' + GIS_JWT_TOKEN, //for object property name, use quoted notation shown in second
    });

    let Pl = {
      stepId: Number(hdnStepID.value),
      lpi: LPIS.LPI,
    };

    let url = hdnUAM.value + 'reservation/addDraftReqGis';
    return this.http.post(url, Pl, { headers: httpheaders });
  }

  pushedCaptureParcelLodgement(LPIS: any) {
    var GIS_JWT_TOKEN = localStorage.getItem('GIS_JWT_TOKEN');
    const hdnUAM: any = document.getElementById('hdnUAM')!;
    const hdnStepID: any = document.getElementById('hdnStepID')!;

    let httpheaders = new HttpHeaders({
      'Content-Type': 'application/json', //If your header name has spaces or any other char not appropriate
      Authorization: 'bearer ' + GIS_JWT_TOKEN, //for object property name, use quoted notation shown in second
    });

    let Pl = {
      stepId: Number(hdnStepID.value),
      lpi: LPIS.LPI,
    };

    let url = hdnUAM.value + 'lodgement/addLdgDraftStepReq';
    return this.http.post(url, Pl, { headers: httpheaders });
  }

  pushedCaptureStreet(LPIS: any) {
    var GIS_JWT_TOKEN = localStorage.getItem('GIS_JWT_TOKEN');
    const hdnUAM: any = document.getElementById('hdnUAM')!;
    const hdnStepID: any = document.getElementById('hdnStepID')!;

    let httpheaders = new HttpHeaders({
      'Content-Type': 'application/json', //If your header name has spaces or any other char not appropriate
      Authorization: 'bearer ' + GIS_JWT_TOKEN, //for object property name, use quoted notation shown in second
    });

    let Pl = {
      stepId: Number(hdnStepID.value),
      lpi: LPIS.LPI,
    };

    let url = hdnUAM.value + 'reservation/addStepOtherData';
    return this.http.post(url, Pl, { headers: httpheaders });
  }

  getUserDataFromExternal(UID: any) {
    let url = this.serviceExternalURL + 'getuserbyID?userID=' + UID;
    return this.http.get(url);
  }

  getLocalMunicDataForProvince(PCode: any) {
    let url =
      this.serviceBaseURL + 'VectorValidation/GetLMDataByProvinceCode/' + PCode;
    return this.http.get(url);
  }

  getWardDataForLocalMunic(LMCode: any) {
    let url =
      this.serviceBaseURL + 'VectorValidation/GetWardDataByLMCode/' + LMCode;
    return this.http.get(url);
  }

  getVDForWardID(WardID: any) {
    let url =
      this.serviceBaseURL + 'VectorValidation/GetVotingDataByWardID/' + WardID;
    return this.http.get(url);
  }

  getLMGeometryFromWMS(PCode: any) {
    let url =
      this.serviceGeoserverURL +
      '&typeName=MDBLayers%3ALocal%20Municipalities&CQL_FILTER=cat_b=%27' +
      PCode +
      '%27';
    return this.http.get(url);
  }

  getWardGeometryFromWMS(LMCode: any) {
    let url =
      this.serviceGeoserverURL +
      '&typeName=MDBLayers%3AWards&CQL_FILTER=wardid=%27' +
      LMCode +
      '%27';
    return this.http.get(url);
  }

  getVDGeometryFromWMS(vdnumber: any) {
    let url =
      this.serviceGeoserverURL +
      '&typeName=MDBLayers%3AVotingDistricts&CQL_FILTER=objectid=%27' +
      vdnumber +
      '%27';
    return this.http.get(url);
  }

  getVDGeometryFromWMSToCheckIntersect(WKT: any) {
    let url = this.serviceGeoserverURL + '&typeName=MDBLayers%3AVotingDistricts&CQL_FILTER=INTERSECTS(geom,%20' + WKT + ')';
    return this.http.get(url);
  }

  getVDGeometryFromWMSToCheckIntersectForOnlySelect(WKT: any) {
    var sgu = this.serviceGeoserverURL.replace('maxFeatures=50', 'maxFeatures=1')
    let url = sgu + '&typeName=MDBLayers%3AVotingDistricts&CQL_FILTER=INTERSECTS(geom,%20' + WKT + ')';
    return this.http.get(url);
  }

  updateVDChanges(arrVDBeans: any) {
    let url = this.serviceBaseURL + 'VectorValidation';

    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    this.http.post(url, arrVDBeans, { headers }).subscribe((response) => {
      console.log(response);
      document.getElementById('AfterApplyChangesClick')?.click();
    });
  }

  getSplitedData(spatialBeansForSplit: any) {
    let url = this.serviceBaseURL + 'VectorSpatial/GetSplittedPolygon';

    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    this.http.post(url, spatialBeansForSplit, { headers }).subscribe((response: any) => {
      //console.log(response);
      document.getElementById('hdnSplitedPolygons')?.setAttribute('value', response[0].split_polygon + '|' + response[1].split_polygon);

      document.getElementById('AfterSplitPolygonClick')?.click();
    });
  }

  saveSplitedData(splitedData: any) {
    let url = this.serviceBaseURL + 'VectorSpatial/SaveSplittedPolygon';

    const headers = new HttpHeaders().set('Content-Type', 'application/json');

    this.http.post(url, splitedData, { headers }).subscribe((response: any) => {
      console.log(response);

    });
  }
}
