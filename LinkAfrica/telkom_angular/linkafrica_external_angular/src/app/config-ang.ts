import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
  })
export class ConfigManager {

    //serviceBaseURL = 'https://localhost:44354/'; // Devlopment Telkom
    //serviceBaseURL = 'http://tectonicsstaging.dedicated.co.za/LAAPI/'; // Staging Environment
    serviceBaseURL = 'http://tectonicstechnologysaprod.dedicated.co.za/LAAPI/'; // Prod Environment

    //serviceExternalURL = 'http://10.1.15.113/MDBAPI/api/Authenticate/'; // 
    serviceExternalURL = 'http://102.210.249.30/MDBAPI/api/Authenticate/'; // 

    //serviceGeoserverURL = 'http://10.1.15.113:8080/GisMDB/MDBLayers/ows?service=WFS&version=1.0.0&request=GetFeature&outputFormat=application%2Fjson&maxFeatures=50&'; // GeoserverURL
    serviceGeoserverURL = 'http://102.210.249.30:8080/GisMDB/MDBLayers/ows?service=WFS&version=1.0.0&request=GetFeature&outputFormat=application%2Fjson&maxFeatures=50&';

    

     //BulkData = 'http://localhost:50083/BulkData.html'; // local
     BulkData = 'http://tectonicstechnologysaprod.dedicated.co.za/App/BulkData.html'; // Prod
     //BulkData = 'http://tectonicsstaging.dedicated.co.za/App/BulkData.html'; // local
}


