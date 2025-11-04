import { Injectable } from '@angular/core';
import { LPIService } from './backendservice';
import { LoaderWaiting } from './loader';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Style from 'ol/style/Style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import Circle from 'ol/style/Circle';
import WKT from 'ol/format/WKT';
import * as olProj from 'ol/proj';
import * as proj4x from 'proj4';
import { register } from 'ol/proj/proj4';
import Overlay from 'ol/Overlay';
import Polygon from 'ol/geom/Polygon';
import LineString from 'ol/geom/LineString';
import * as Sphere from 'ol/sphere';
import DrawVectorForSaerch, {
  createBox,
  createRegularPolygon,
} from 'ol/interaction/draw';
import Observable from 'ol/Observable';
import { ErrorMessage } from './errormessage';
import { MapGeometryAction } from './mapgeometryaction';
import { QueryAction } from './queryaction';
import { VDBeans, WardDetail, SpatialBeansForSplit, SplitedData } from './AppBeans';

@Injectable({
  providedIn: 'root',
})
export class SearchByDrawing {
  mmap: any;
  draw: any;
  drawingToolName: any = '';
  vectorDrawingLayer: any;
  sourceDrawing: any;

  vectorVDLayer: any;
  sourceVD: any;
  boxVDData: any = [];
  wardDetails: any = new WardDetail('0', '0', '0', '0', '0');
  isSplitToolSelected: boolean = false;
  isSplitToolPolygonSelected: boolean = false;

  selectedDataforSplited: any = []; 

  sketch: any;

  constructor(
    private lpiserver: LPIService,
    private loader: LoaderWaiting,
    private mga: MapGeometryAction,
    private qa: QueryAction
  ) {}

  initSearchByDrawing(_map: any) {
    this.mmap = _map;
    this.CreateVDLayerForSelected();
    this.CreateVectorLayerForDrawing();
  }

  drawingStart(buttonName: any) {
    this.isSplitToolSelected = false;
    this.isSplitToolPolygonSelected = false;
    switch (buttonName) {
      case 'freedrawingpolygon':
        this.StopInteraction();
        this.drawingToolName = 'Polygon';
        this.AddButtonInteraction('Polygon', null, false);
        break;
      case 'rectangle':
        this.StopInteraction();
        this.drawingToolName = 'Circle';
        this.AddButtonInteraction('Circle', createBox(), false);
        break;
        case 'freedrawingpolygonline':
          this.isSplitToolPolygonSelected = true;
          this.StopInteraction();
          this.drawingToolName = 'Circle';
          this.AddButtonInteraction('Circle', createBox(), false);
          break;
      case 'line':
        this.isSplitToolSelected = true;
        this.StopInteraction();
        this.drawingToolName = 'LineString';
        this.AddButtonInteraction('LineString', null, false);
        break;
      case 'none':
        this.StopInteraction();
        this.drawingToolName = 'None';
        this.clearSelectedVDFeature();
        break;
      case 'clear':
        var features = this.vectorDrawingLayer.getSource().getFeatures();
        features.forEach((feature: any) => {
          this.vectorDrawingLayer.getSource().removeFeature(feature);
        });
        this.clearSelectedVDFeature();
        break;
    }
    if((buttonName !== 'line') && (buttonName !== 'freedrawingpolygonline')){
        setTimeout(() => {
          console.log('team-1', buttonName);
          this.FetchDataFromVectorLayer();
        }, 10000);
    }

    if(buttonName === 'freedrawingpolygonline'){
      setTimeout(() => {
        console.log('team-2', buttonName);
         this.FetchDataFromVectorLayerForOnlySelect();
      }, 10000);
    }

    if(buttonName === 'line'){
      setTimeout(() => {
        console.log('team-3', buttonName);
         this.FetchDataFromVectorLayerForLine();
      }, 10000);
    }


  }

  FetchDataFromVectorLayer() {
    var features = this.vectorDrawingLayer.getSource().getFeatures();
    if (features.length > 0) {
      var feature: any = features[0];
      let geometry = feature.getGeometry();
      let allCoordinates: any = [];

      var wkt4326 = this.getPolygonfeatureWKTin4326(geometry);

      this.lpiserver.getVDGeometryFromWMSToCheckIntersect(wkt4326).subscribe((VDData: any) => {
          var hdnSelectedWardID: any =
            document.getElementById('hdnSelectedWardID')!;
          let fetureProps: any = [];
          let fetureGeomSelected: any = [];
          console.log(VDData.features.length);
          VDData.features.forEach(function (fet: any) {
            if (fet.properties.wardid == hdnSelectedWardID.value) {
            var cordData = fet.geometry.coordinates;
            var fetProp = fet.properties;
            fetureGeomSelected.push({ cord: cordData, prop: fetProp });
            fetureProps.push(fetProp);
            }
          });
          console.log(fetureGeomSelected.length);
          setTimeout(() => {
            this.createWKTandDraw(fetureGeomSelected);
            this.renderSelectedVD(fetureProps);
          }, 300);
        });
    }
    this.StopInteraction();
    this.drawingToolName = 'None';
    this.loader.StopLoader();
  }

  FetchDataFromVectorLayerForLine() {
    var featuresLine = this.vectorDrawingLayer.getSource().getFeatures();
    var featuresPolygon = this.vectorVDLayer.getSource().getFeatures();

     if (featuresLine.length === 1 && featuresPolygon.length === 1) {

        var line_feature: any = featuresLine[0];
        let line_geometry = line_feature.getGeometry();

        var polygon_feature: any = featuresPolygon[0];
        let polygon_geometry = polygon_feature.getGeometry(); 

        var LINE_wkt = this.getLineGonfeatureWKTin4326(line_geometry)

        var polygon_wkt = this.getPolygonfeatureWKTin4326(polygon_geometry);

        var dataa = new SpatialBeansForSplit();
        dataa.LineWKT = LINE_wkt;
        dataa.PolygonWKT = polygon_wkt;  

        this.lpiserver.getSplitedData(dataa);

     }
     else{

     }
    
    
    this.StopInteraction();
    this.drawingToolName = 'None';
    this.loader.StopLoader();
  }

  FetchDataFromVectorLayerForOnlySelect() {
    this.selectedDataforSplited = [];
    var features = this.vectorDrawingLayer.getSource().getFeatures();
    if (features.length > 0) {
      var feature: any = features[0];
      let geometry = feature.getGeometry();
      let allCoordinates: any = [];      
      
      var wkt4326 = this.getPolygonfeatureWKTin4326(geometry)
     var localThis = this;

      this.lpiserver.getVDGeometryFromWMSToCheckIntersectForOnlySelect(wkt4326).subscribe((VDData: any) => {
          var hdnSelectedWardID: any =
            document.getElementById('hdnSelectedWardID')!;
          let fetureProps: any = [];
          let fetureGeomSelected: any = [];
          VDData.features.forEach(function (fet: any) {
            //if (fet.properties.wardid == hdnSelectedWardID.value) {
            var cordData = fet.geometry.coordinates;
            var fetProp = fet.properties;
            fetureGeomSelected.push({ cord: cordData, prop: fetProp });
            localThis.selectedDataforSplited.push({ cord: cordData, prop: fetProp });
            fetureProps.push(fetProp);
            //}
          });
          console.log(fetureGeomSelected.length);
          setTimeout(() => {
            this.createWKTandDrawForOnlySelect(fetureGeomSelected);
          }, 300);
        });
    }
    this.StopInteraction();
    this.drawingToolName = 'None';
    this.loader.StopLoader();
  }

  createWKTandDrawForOnlySelect(featureGeom: any) {
    var wkt: any;
    featureGeom.forEach((boj: any) => {
      var element1 = boj.cord;
      var prop_vdnumber = boj.prop.vdnumber;
      element1.forEach((element2: any) => {
        element2.forEach((element3: any) => {
          wkt = 'POLYGON ((';
          element3.forEach((element4: any) => {
            if (wkt == 'POLYGON ((') {
              wkt = wkt + element4[0] + ' ' + element4[1];
            } else {
              wkt = wkt + ', ' + element4[0] + ' ' + element4[1];
            }
          });
          wkt = wkt + '))';
          this.mga.drawWKTByLayerObjectForOnlySelect(wkt, this.vectorVDLayer, prop_vdnumber);
        });
      });
    });
  }

  getPolygonfeatureWKTin4326(geometry: any){

    let allCoordinates: any = [];
    geometry.getCoordinates().forEach((coords: any) => {
      allCoordinates = allCoordinates.concat(coords);
    });

    //console.log(allCoordinates);

    var wkt = 'POLYGON ((';
    allCoordinates.forEach((element: any) => {
      if (wkt == 'POLYGON ((') {
        wkt = wkt + element[0] + ' ' + element[1];
      } else {
        wkt = wkt + ', ' + element[0] + ' ' + element[1];
      }
    });

    wkt = wkt + '))';
    //console.log(wkt);

    const format = new WKT();
    const feature4326 = format.readFeature(wkt, {
      dataProjection: 'EPSG:3857', //this.SourceProjection,
      featureProjection: 'EPSG:4326', //this.DestinationProjection,
    });
    //console.log(feature4326);

    let geometry4326: any = feature4326.getGeometry();
    let allCoordinates4326: any = [];

    geometry4326.getCoordinates().forEach((coords: any) => {
      allCoordinates4326 = allCoordinates4326.concat(coords);
    });

    //console.log(allCoordinates4326);
    var wkt4326 = 'POLYGON ((';
    allCoordinates4326.forEach((element: any) => {
      if (wkt4326 == 'POLYGON ((') {
        wkt4326 = wkt4326 + element[0] + ' ' + element[1];
      } else {
        wkt4326 = wkt4326 + ', ' + element[0] + ' ' + element[1];
      }
    });

    wkt4326 = wkt4326 + '))';

    return wkt4326;
  }

  getLineGonfeatureWKTin4326(geometry: any){

    let allCoordinates: any = [];
    geometry.getCoordinates().forEach((coords: any) => {
      allCoordinates = allCoordinates.concat(coords[0] + '|' + coords[1]);
    });

    //console.log(allCoordinates);

    var wkt = 'LINESTRING (';
    allCoordinates.forEach((crd: any) => {
      var element = crd.split('|');
      if (wkt == 'LINESTRING (') {
        wkt = wkt + element[0] + ' ' + element[1];
      } else {
        wkt = wkt + ', ' + element[0] + ' ' + element[1];
      }
    });

    wkt = wkt + ')';
    //console.log(wkt);

    const format = new WKT();
    const feature4326 = format.readFeature(wkt, {
      dataProjection: 'EPSG:3857', //this.SourceProjection,
      featureProjection: 'EPSG:4326', //this.DestinationProjection,
    });
    //console.log(feature4326);

    let geometry4326: any = feature4326.getGeometry();
    let allCoordinates4326: any = [];

    geometry4326.getCoordinates().forEach((coords: any) => {
      allCoordinates4326 = allCoordinates4326.concat(coords[0] + '|' + coords[1]);
    });

    //console.log(allCoordinates4326);
    var wkt4326 = 'LINESTRING (';
    allCoordinates4326.forEach((cord: any) => {
      var element = cord.split('|');
      if (wkt4326 == 'LINESTRING (') {
        wkt4326 = wkt4326 + element[0] + ' ' + element[1];
      } else {
        wkt4326 = wkt4326 + ', ' + element[0] + ' ' + element[1];
      }
    });

    wkt4326 = wkt4326 + ')';

    return wkt4326;
  }

  renderSelectedVD(Attata: any) {
    var thiss = this;

    Attata.forEach(function (fet: any) {
      thiss.boxVDData.push({
        VD_Number: fet.vdnumber,
        RegPop: fet.regpop,
      });
    });

    var tblSelVD: any = document.getElementById('tblSelVD')!;
    if (thiss.boxVDData.length > 0) {
      tblSelVD.style.display = 'block';
    } else {
      tblSelVD.style.display = 'none';
    }
    var TotalRegPop: number = 0;
    thiss.boxVDData.forEach(function (vet: any) {
      TotalRegPop = TotalRegPop + parseInt(vet.RegPop);
    });

    var tblWardDetails: any = document.getElementById('tblWardDetails')!;
    var hdnSelectedWardID: any = document.getElementById('hdnSelectedWardID')!;
    if (hdnSelectedWardID.value != '0') {
      tblWardDetails.style.display = 'block';

      this.qa.wardDataForDeLimitation.forEach(function (sset: any) {
        if (hdnSelectedWardID.value == sset.wardid) {
          thiss.wardDetails = new WardDetail(
            TotalRegPop.toString(),
            sset.normvalue,
            sset.norm_min,
            sset.norm_max,
            sset.wards2019
          );
        }
      });
    } else {
      tblWardDetails.style.display = 'none';
    }
  }

  createWKTandDraw(featureGeom: any) {
    debugger;
    var wkt: any;
    featureGeom.forEach((boj: any) => {
      var element1 = boj.cord;
      var prop_vdnumber = boj.prop.vdnumber;
      element1.forEach((element2: any) => {
        element2.forEach((element3: any) => {
          wkt = 'POLYGON ((';
          element3.forEach((element4: any) => {
            if (wkt == 'POLYGON ((') {
              wkt = wkt + element4[0] + ' ' + element4[1];
            } else {
              wkt = wkt + ', ' + element4[0] + ' ' + element4[1];
            }
          });
          wkt = wkt + '))';
          this.mga.drawWKTByLayerObject(wkt, this.vectorVDLayer, prop_vdnumber);
        });
      });
    });
  }

  StopInteraction() {
    this.mmap.removeInteraction(this.draw);
    this.sourceDrawing.clear();
    this.drawingToolName = '';
    this.draw = null;
  }

  AddButtonInteraction(type: any, geometryFunction: any, freehand: any) {
    //this.StopInteraction();
    this.draw = new DrawVectorForSaerch({
      source: this.sourceDrawing,
      type: type,
      freehand: freehand,
      geometryFunction: geometryFunction,
      style: new Style({
        fill: new Fill({
          color: 'rgba(255, 255, 255, 0.2)',
        }),
        stroke: new Stroke({
          color: 'rgba(0, 0, 0, 0.5)',
          lineDash: [10, 10],
          width: 2,
        }),
        image: new Circle({
          radius: 5,
          stroke: new Stroke({
            color: 'rgba(0, 0, 0, 0.7)',
          }),
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)',
          }),
        }),
      }),
    });
    this.mmap.addInteraction(this.draw);
    var that = this;
    this.draw.on('drawstart', function (evt: any) {
      // set sketch
      that.sketch = evt.feature;
    });
    this.draw.on('drawend', function (evt: any) {
      that.sketch = null;
      const geom = evt.target;
      if(!that.isSplitToolSelected){
        that.loader.StartLoader();
      }
    });
  }

  CreateVectorLayerForDrawing() {
    this.sourceDrawing = new VectorSource();

    this.vectorDrawingLayer = new VectorLayer({
      source: this.sourceDrawing,
      style: new Style({
        fill: new Fill({
          color: 'rgba(143, 33, 33, 0.2)',
        }),
        stroke: new Stroke({
          color: '#ff0000',
          width: 2,
        }),
        image: new Circle({
          radius: 7,
          fill: new Fill({
            color: '#8c2121',
          }),
        }),
      }),
    });
    this.mmap.addLayer(this.vectorDrawingLayer);
  }

  CreateVDLayerForSelected() {
    this.sourceVD = new VectorSource();

    this.vectorVDLayer = new VectorLayer({
      source: this.sourceVD,
      style: new Style({
        fill: new Fill({
          color: 'rgba(143, 33, 33, 0.2)',
        }),
        stroke: new Stroke({
          color: '#8c2121',
          width: 2,
        }),
        image: new Circle({
          radius: 7,
          fill: new Fill({
            color: '#8c2121',
          }),
        }),
      }),
    });
    this.mmap.addLayer(this.vectorVDLayer);
  }

  clearSelectedVDFeature() {
    var features = this.vectorVDLayer.getSource().getFeatures();
    features.forEach((feature: any) => {
      this.vectorVDLayer.getSource().removeFeature(feature);
    });

    this.boxVDData = [];
    var tblSelVD = document.getElementById('tblSelVD')!;
    if (this.boxVDData.length > 0) {
      tblSelVD.style.display = 'block';
    } else {
      tblSelVD.style.display = 'none';
    }
    if (this.boxVDData.length === 0) {
      this.wardDetails = new WardDetail('0', '0', '0', '0', '0');
      var tblWardDetails: any = document.getElementById('tblWardDetails')!;
      tblWardDetails.style.display = 'none';
    }
  }

  UnSelectFeature(param: any) {
    var features = this.vectorVDLayer.getSource().getFeatures();
    features.forEach((feature: any) => {
      //debugger;
      const properties = feature.getProperties();
      if (properties.vdNumber === param) {
        this.vectorVDLayer.getSource().removeFeature(feature);
      }
    });
    var boxVDData_Dummy: any = [];
    this.boxVDData.forEach(function (vet: any) {
      if (vet.VD_Number != param) {
        boxVDData_Dummy.push(vet);
      }
    });
    this.boxVDData = [];
    this.boxVDData = Array.from(boxVDData_Dummy);

    if (this.boxVDData.length === 0) {
      this.wardDetails = new WardDetail('0', '0', '0', '0', '0');
      var tblWardDetails: any = document.getElementById('tblWardDetails')!;
      var tblSelVD = document.getElementById('tblSelVD')!;
      tblWardDetails.style.display = 'none';
      tblSelVD.style.display = 'none';
    } else {
      var TotalRegPop: number = 0;
      this.boxVDData.forEach(function (vet: any) {
        TotalRegPop = TotalRegPop + parseInt(vet.RegPop);
      });
      this.wardDetails.TotalRegPop = TotalRegPop;
    }
  }

  AcceptChange(param: any) {
    // let arrLocalVDBeans: VDBeans[] = [];
    // console.log(this.boxVDData.length);
    // console.log(JSON.stringify(this.boxVDData));
    var hdnSelectedWardID: any = document.getElementById('hdnSelectedWardID')!;

    //console.log(hdnSelectedWardID.value);
   

    var dataa = new VDBeans();
    dataa.VDData = JSON.stringify(this.boxVDData);
    dataa.SelectedWardID = hdnSelectedWardID.value;   
   

    this.lpiserver.updateVDChanges(dataa);
    //this.wardDetails
    //alert('To Do ');
  }

  DrawSplitedPolygon(par: any){
    var features = this.vectorVDLayer.getSource().getFeatures();
    features.forEach((feature: any) => {
      this.vectorVDLayer.getSource().removeFeature(feature);
    });

    var wkts = par.split('|');
    this.mga.draSplitedWKTByLayerObject(wkts[0], this.vectorVDLayer, 'A');
    this.mga.draSplitedWKTByLayerObject(wkts[1], this.vectorVDLayer, 'B');
    
    
    var cordData = this.selectedDataforSplited[0].cord;
    var fetProp = this.selectedDataforSplited[0].prop;

    this.selectedDataforSplited = [];
    this.selectedDataforSplited.push({ cord: cordData, prop: fetProp, splitedwkt: wkts });    
    

    var boxName = document.getElementById('boxSplitGrid')!;
    boxName.style.display = 'block';


    document.getElementById('txtVDID_A')?.setAttribute('value', this.selectedDataforSplited[0].prop.vdnumber);
    document.getElementById('txtVDID_B')?.setAttribute('value', this.selectedDataforSplited[0].prop.vdnumber);

    document.getElementById('txtWardID_A')?.setAttribute('value', this.selectedDataforSplited[0].prop.wardid);
    document.getElementById('txtWardID_B')?.setAttribute('value', this.selectedDataforSplited[0].prop.wardid);

    document.getElementById('txtWardNumber_A')?.setAttribute('value', this.selectedDataforSplited[0].prop.wardno);
    document.getElementById('txtWardNumber_B')?.setAttribute('value', this.selectedDataforSplited[0].prop.wardno);

    document.getElementById('lblRegPopReminder')?.setAttribute('value', this.selectedDataforSplited[0].prop.regpop);

   

  }

  KeyDownRegPop(event: KeyboardEvent){
    const inputValue = (event.target as HTMLInputElement).value;
    const numericInput = inputValue.replace(/[^0-9]/g, '');
    (event.target as HTMLInputElement).value = numericInput;

    var txtRegPop_A: any = document.getElementById('txtRegPop_A');
    var txtRegPop_B: any = document.getElementById('txtRegPop_B');
    var lblRegPopReminder: any = document.getElementById('lblRegPopReminder');    

    var finalValue = parseInt(this.selectedDataforSplited[0].prop.regpop) || 0;
    var firstValue = parseInt(txtRegPop_A.value) || 0;
    var secondValue = parseInt(txtRegPop_B.value) || 0;
    const difference = finalValue - (firstValue + secondValue);
    lblRegPopReminder.value = difference;    
    if(difference === 0){      
      document.getElementById('btnSaveSplit')?.removeAttribute('disabled');
    }
    else{
      document.getElementById('btnSaveSplit')?.setAttribute('disabled', 'true');
    }
  }

  

  SaveSplitedPolygon(){

    var cordData = this.selectedDataforSplited[0].cord;
    var fetProp = this.selectedDataforSplited[0].prop;
    var wkts = this.selectedDataforSplited[0].splitedwkt;

    var txtRegPop_A: any = document.getElementById('txtRegPop_A');
    var txtRegPop_B: any = document.getElementById('txtRegPop_B');  
    var RegPop_A = parseInt(txtRegPop_A.value) || 0;
    var RegPop_B = parseInt(txtRegPop_B.value) || 0;
    var newRegPopValues: any ={val_A: RegPop_A, val_B: RegPop_B};

    var txtWardNumber_A: any = document.getElementById('txtWardNumber_A');
    var txtWardNumber_B: any = document.getElementById('txtWardNumber_B');  

    var WardNumber_A = parseInt(txtWardNumber_A.value) || 0;
    var WardNumber_B = parseInt(txtWardNumber_B.value) || 0;

    var newPolygonWardNumber: any ={WardNum_A: WardNumber_A, WardNum_B: WardNumber_B};
    

    var dataforSplited = [];
    dataforSplited.push({ prop: fetProp, splitedwkt: wkts, regPopValues: newRegPopValues, polygonWardNumber: newPolygonWardNumber }); 
 
    const jsonString = JSON.stringify(dataforSplited);

    var dataa = new SplitedData();
    dataa.DataOutput = jsonString;
    console.log(dataa, "SaveSplitedPolygonBefore");
    this.lpiserver.saveSplitedData(dataa);
    console.log(dataa, "SaveSplitedPolygonAfter");

    setTimeout(() => {
      //this.GetDataForMap();
    }, 3000);
    
  }

 

  CancelSplitedPolygon(){
    this.clearSelectedVDFeature();

    var boxName = document.getElementById('boxSplitGrid')!;
    boxName.style.display = 'none';

    this.selectedDataforSplited = [];
  }
}




