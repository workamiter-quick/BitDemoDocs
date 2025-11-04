import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LPIS_ARR, LOCATION_ARR } from './ApplicationsBeans';

import { LPIService } from './backendservice';
import { ErrorMessage } from './errormessage';
import { LoaderWaiting } from './loader';

import { MapGeometryAction } from './mapgeometryaction';

@Injectable({
  providedIn: 'root',
})
export class QueryAction {
  mmap: any;
  LPI_RES: string[];
  lpiDataWithGeometry: any;
  lmDataWithGeometry: any;
  lmDataForDocker: any;
  wardDataWithGeometry: any;
  wardDataForDocker: any;
  wardDataForDeLimitation: any;
  votingDataWithGeometry: any;
  votingDataForDocker: any;
  LpiRelationship: any;
  constructor(
    private route: ActivatedRoute,
    private lpiserver: LPIService,
    private mga: MapGeometryAction,
    private loader: LoaderWaiting,
    private errMsg: ErrorMessage
  ) {
    this.LPI_RES = [];
  }

  actionOnQueryString(_map: any) {
    this.mmap = _map;
    this.validateQueryParameter();
    this.mga.initClass(this.mmap);
  }

  validateQueryParameter() {
    // var UID = this.getQueryStringValueByKey('UID')!;
    // const hdnUID = document.getElementById('hdnUID')!;
    // hdnUID.setAttribute('value', UID);

    // var YEAR = this.getQueryStringValueByKey('YEAR')!;
    // const hdnYear = document.getElementById('hdnYear')!;
    // hdnYear.setAttribute('value', YEAR);

    setTimeout(() => {
      //this.GetUserLocation();
      //this.GetDataForMap();
    }, 200);
  }

  

 

  

  getQueryStringValueByKey(par: any) {
    var variableName = par;
    var queryLength = this.route.snapshot.queryParamMap.keys.length;
    for (var t = 0; t < queryLength; t++) {
      if (
        par.toUpperCase() ==
        this.route.snapshot.queryParamMap.keys[t].toUpperCase()
      ) {
        variableName = this.route.snapshot.queryParamMap.keys[t];
      }
    }
    var ParValue = this.route.snapshot.queryParamMap.get(variableName);
    return ParValue;
  }

  

  

  GetUrl() {
    return this.route.url;
  }
}
