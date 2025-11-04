import { Injectable } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import Map from 'ol/map';
import View from 'ol/View';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Style from 'ol/style/Style';
import Text from 'ol/style/Text';
import Strok from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import CircleStyle from 'ol/style/Circle';
import MousePosition from 'ol/control/MousePosition';
import { createStringXY } from 'ol/coordinate';
import Icon from 'ol/style/Icon';
import BingMaps from 'ol/source/BingMaps';
import OSM from 'ol/source/OSM';
import * as olProj from 'ol/proj';
import TileLayer from 'ol/layer/Tile';
import TileWMS from 'ol/source/TileWMS';
import { ScaleLine, defaults as defaultControls } from 'ol/control';
import 'ol/ol.css';
import WKT from 'ol/format/WKT';
import { map } from 'rxjs';
import { LPIService } from './backendservice';

@Injectable({
  providedIn: 'root',
})
export class MapGeometryAction {
  mmap: any;
  vectorMemoryLayer: any;
  wktVertax: any = [];
  wmsConfigData: any;
  currentBaseMap: string = 'OSM';

  constructor(private lpiserver: LPIService) { }
  initClass(_map: any) {
    this.mmap = _map;
    this.vectorMemoryLayer = new VectorLayer({
      className: 'V___Parcel1',
      source: new VectorSource({}),
    });
    this.mmap.addLayer(this.vectorMemoryLayer);
    this.GetLayersFromDB();
  }

  drawLMWKT(data: any, hashColor: any, codeData: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    var textStyle = new Text({
      text: codeData,
      textAlign: 'center',
      textBaseline: 'middle',
      font: '12px Arial',
      fill: new Fill({
        color: 'black',
      }),
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#000000',
          width: 3,
        }),
        fill: new Fill({
          color: hashColor,
        }),
        text: textStyle,
      })
    );
    feature.setProperties({ CODE: codeData, locationLevel: 'Province' });
    featureArray.push(feature);
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.vectorMemoryLayer.setOpacity(0.6);
  }

  drawWardWKT(data: any, hashColor: any, codeData: any, wardno: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    var textStyle = new Text({
      text: wardno,
      textAlign: 'center',
      textBaseline: 'middle',
      font: '18px Arial',
      fill: new Fill({
        color: '#800080',
      }),
      backgroundFill: new Fill({
        color: 'white', // Specify the color for the background
      }),
      padding: [1, 1, 1, 1],
      offsetX: 15,
      offsetY: 15,
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#000000',
          width: 3,
        }),
        fill: new Fill({
          color: hashColor,
        }),
        text: textStyle,
      })
    );
    feature.setProperties({
      CODE: codeData,
      locationLevel: 'LocalMunicipality',
    });
    featureArray.push(feature);
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.vectorMemoryLayer.setOpacity(0.6);
  }

  drawVDWKT(
    data: any,
    hashColor: any,
    codeData: any,
    regPop: any,
    borderColor: any
  ) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var VDWKT = data; //.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(VDWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    var textStyle = new Text({
      text: regPop.toString() + ' (' + codeData + ')',
      textAlign: 'center',
      textBaseline: 'middle',
      font: '12px Arial',
      fill: new Fill({
        color: '#800080',
      }),
      backgroundFill: new Fill({
        color: 'white', // Specify the color for the background
      }),
      padding: [1, 1, 1, 1],
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: borderColor,
          width: 1,
        }),
        fill: new Fill({
          color: hashColor,
        }),
        text: textStyle,
      })
    );
    feature.setProperties({
      CODE: codeData.toString(),
      REGPOP: regPop.toString(),
      locationLevel: 'Voting District',
    });
    featureArray.push(feature);
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.vectorMemoryLayer.setOpacity(0.6);
  }

  zoomToPolygon() {
    var extent = this.vectorMemoryLayer.getSource().getExtent();
    this.mmap.getView().fit(extent, this.mmap.getSize());

    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    this.mmap.getView().setCenter(cen);
    this.mmap.getView().setZoom(zoo);
  }

  drawWKT(data: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    data.forEach(function (value: any) {
      const format = new WKT();
      var LPIWKT = value.WKT; //.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;

      that.wktVertax.push(value.CENTER_X + ' ' + value.CENTER_Y);
      const feature = format.readFeature(LPIWKT, {
        dataProjection: 'EPSG:4326', //this.SourceProjection,
        featureProjection: 'EPSG:3857', //this.DestinationProjection,
      });
      feature.setStyle(
        new Style({
          stroke: new Strok({
            color: '#00ff00',
            width: 8,
          }),
        })
      );
      extent = feature.getGeometry()?.getExtent();
      featureArray.push(feature);
    });
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);

    this.zoomToPolygon();
  }

  drawChildParentWKT(data: any) {
    var extent: any;
    let featureArray: any = [];
    for (let Val in data) {
      try {
        var Mdata = data[Val];
        var RecordID = Mdata.RecordID;
        var RecordName = Mdata.RecordName;
        var that = this;
        var WKTData = 'POLYGON ((';
        Mdata.lstChildParentCordData.forEach(function (obj: any) {
          var XCoord = obj.XCoord;
          var YCoord = obj.YCoord;
          var LOProj = obj.LOProj;
          var newCoord: any;
          switch (LOProj.toUpperCase()) {
            case 'WG 33':
              var epsg = 'EPSG:2055';
              newCoord = olProj.transform(
                [parseFloat(XCoord), parseFloat(YCoord)],
                epsg,
                'EPSG:4326'
              );
              WKTData =
                WKTData +
                newCoord[1].toString() +
                ' -' +
                newCoord[0].toString() +
                ', ';

              break;
            case 'WG 25':
              break;
            case 'WG 31':
              var epsg = 'EPSG:2054';
              newCoord = olProj.transform(
                [parseFloat(XCoord), parseFloat(YCoord)],
                epsg,
                'EPSG:4326'
              );
              break;
          }
        });
        WKTData = WKTData + '#';
        WKTData = WKTData.replace(', #', '))');

        var featureColor: string = RecordName == 'C' ? 'af983a' : '3aaf53';
        var featureWidth: number = RecordName == 'C' ? 4 : 8;

        const format = new WKT();
        const feature = format.readFeature(WKTData, {
          dataProjection: 'EPSG:4326', //this.SourceProjection,
          featureProjection: 'EPSG:3857', //this.DestinationProjection,
        });
        feature.setStyle(
          new Style({
            stroke: new Strok({
              color: featureColor,
              width: featureWidth,
            }),
          })
        );
        extent = feature.getGeometry()?.getExtent();
        featureArray.push(feature);
      } catch { }
    }
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.mmap.getView().fit(extent);
    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    this.mmap.getView().setCenter(cen);
    this.mmap.getView().setZoom(zoo - 1);
  }

  drawWKTLocation(data: any, IsZoom: boolean = true) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    data.forEach(function (value: any) {
      const format = new WKT();
      var LPIWKT = value.WKT; //.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;

      //that.wktVertax.push(value.CENTER_X + ' ' + value.CENTER_Y);
      const feature = format.readFeature(LPIWKT, {
        dataProjection: 'EPSG:4326', //this.SourceProjection,
        featureProjection: 'EPSG:3857', //this.DestinationProjection,
      });
      feature.setStyle(
        new Style({
          stroke: new Strok({
            color: '#800080',
            width: 2,
          }),
        })
      );
      extent = feature.getGeometry()?.getExtent();
      featureArray.push(feature);
    });
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    if (IsZoom) {
      if (data.length == 1) {
        this.mmap.getView().fit(extent);
        var cen: any = this.mmap.getView().getCenter();
        var zoo: any = this.mmap.getView().getZoom();
        this.mmap.getView().setCenter(cen);
        this.mmap.getView().setZoom(zoo - 2);
      }
    }
  }

  drawSingleWKT(data: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#00ffe9',
          width: 5,
        }),
      })
    );
    extent = feature.getGeometry()?.getExtent();
    featureArray.push(feature);
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.mmap.getView().fit(extent);
    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    this.mmap.getView().setCenter(cen);
    this.mmap.getView().setZoom(zoo - 2);
    setTimeout(() => {
      //this.ClearAllMemoryLayerGeometry();
    }, 2000000);
  }

  drawSingleStreetWKT(data: any, CENTER_X: any, CENTER_Y: any) {
    this.ClearAllMemoryLayerGeometry();
    var extent: any;
    let featureArray: any = [];
    var that = this;
    this.wktVertax.push(CENTER_X + ' ' + CENTER_Y);
    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#0000ff',
          width: 5,
        }),
      })
    );
    extent = feature.getGeometry()?.getExtent();
    featureArray.push(feature);
    this.vectorMemoryLayer.getSource().addFeatures(featureArray);
    this.mmap.getView().fit(extent);
    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    this.mmap.getView().setCenter(cen);
    this.mmap.getView().setZoom(zoo - 1);
    setTimeout(() => {
      this.ClearAllMemoryLayerGeometry();
    }, 2000000);
  }

  zoomToXY(XCoord: string, YCoord: string) {
    try {
      this.ClearAllMemoryLayerGeometry();
      var extent: any;
      var wktt = 'POINT (' + XCoord + ' ' + YCoord + ')';
      let featureArray: any = [];
      const format = new WKT();
      const feature = format.readFeature(wktt, {
        dataProjection: 'EPSG:4326', //this.SourceProjection,
        featureProjection: 'EPSG:3857', //this.DestinationProjection,
      });
      feature.setStyle(
        new Style({
          stroke: new Strok({
            color: '#0000ff',
            width: 5,
          }),
          image: new Icon({
            src: 'assets/Icon/AddressSearch_Marker.png',
            anchor: [-0.01, 1], // bottom center anchor
            scale: 1.2,
          }),
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)',
          }),
        })
      );
      extent = feature.getGeometry()?.getExtent();
      featureArray.push(feature);
      this.vectorMemoryLayer.getSource().addFeatures(featureArray);
      this.mmap.getView().fit(extent);
      var cen: any = this.mmap.getView().getCenter();
      var zoo: any = this.mmap.getView().getZoom();
      this.mmap.getView().setCenter(cen);
      this.mmap.getView().setZoom(zoo - 10);
      setTimeout(() => {
        //this.ClearAllMemoryLayerGeometry();
      }, 180000);
    } catch (e) {
      console.log(e);
    }
  }

  zoomToXYGreen(XCoord: string, YCoord: string) {
    try {
      //this.ClearAllMemoryLayerGeometry();
      var extent: any;
      var wktt = 'POINT (' + XCoord + ' ' + YCoord + ')';
      let featureArray: any = [];
      const format = new WKT();
      const feature = format.readFeature(wktt, {
        dataProjection: 'EPSG:4326', //this.SourceProjection,
        featureProjection: 'EPSG:3857', //this.DestinationProjection,
      });
      feature.setStyle(
        new Style({
          stroke: new Strok({
            color: '#0000ff',
            width: 5,
          }),
          image: new Icon({
            src: 'assets/Icon/AddressSearch_Marker_Green.png',
            anchor: [-0.01, 1], // bottom center anchor
            scale: 1.2,
          }),
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)',
          }),
        })
      );
      extent = feature.getGeometry()?.getExtent();
      featureArray.push(feature);
      this.vectorMemoryLayer.getSource().addFeatures(featureArray);
      this.mmap.getView().fit(extent);
      var cen: any = this.mmap.getView().getCenter();
      var zoo: any = this.mmap.getView().getZoom();
      this.mmap.getView().setCenter(cen);
      this.mmap.getView().setZoom(zoo - 10);
      setTimeout(() => {
        //this.ClearAllMemoryLayerGeometry();
      }, 180000);
    } catch (e) {
      console.log(e);
    }
  }

  XYDrawForPath(XCoord: string, YCoord: string) {
    try {

      var extent: any;
      var wktt = 'POINT (' + XCoord + ' ' + YCoord + ')';
      let featureArray: any = [];
      const format = new WKT();
      const feature = format.readFeature(wktt, {
        dataProjection: 'EPSG:4326', //this.SourceProjection,
        featureProjection: 'EPSG:3857', //this.DestinationProjection,
      });
      feature.setStyle(
        new Style({
          stroke: new Strok({
            color: '#0000ff',
            width: 5,
          }),
          // image: new CircleStyle({
          //   radius: 9,
          //   fill: new Fill({
          //     color: '#4332ff',
          //   }),
          // }),
          image: new Icon({
            src: 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="red" viewBox="0 0 24 24"><path d="M6 2v20l6-6 6 6V2z"/></svg>',
            anchor: [0.5, 1], // bottom-center anchor
            rotateWithView: true,
            scale: 1.2
          }),
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)',
          }),
        })
      );
      extent = feature.getGeometry()?.getExtent();
      featureArray.push(feature);
      this.vectorMemoryLayer.getSource().addFeatures(featureArray);

    } catch (e) {
      console.log(e);
    }
  }

  ClearAllMemoryLayerGeometry() {
    this.vectorMemoryLayer.getSource().clear();
  }

  GetMapCurrentStatus() {
    let mapStatus = '';
    let visibleMapLayers: string = '';
    this.wmsConfigData.forEach(function (valChild: any) {
      let cchk = <HTMLInputElement>(
        document.getElementById('O___' + valChild.LAYER_NAME.split(':')[1])
      );
      if (cchk.checked) {
        visibleMapLayers = visibleMapLayers + '|' + valChild.OBJECTID;
      }
    });
    var BaseLayername = this.currentBaseMap;
    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    mapStatus =
      'Z=' +
      zoo +
      '&X=' +
      cen[0] +
      '&Y=' +
      cen[1] +
      '&L=' +
      visibleMapLayers +
      '&B=' +
      BaseLayername;
    return mapStatus;
  }

  GetLayersFromDB() {
    let localThis = this;
    this.lpiserver.getLMData().subscribe((data: any) => {
      localThis.wmsConfigData = data;
    });
  }

  SetCurrentBaseMap(par: any) {
    this.currentBaseMap = par;
  }

  SetMapCurrentStatus(bookmarkMapStatus: any) {
    var valStatus = bookmarkMapStatus.split('&');

    var baseMap = valStatus[4].split('=')[1];
    var lyrs = this.mmap.getLayers().getArray();
    for (let i = 0; i < lyrs.length; i++) {
      var lly = lyrs[i];
      var tCls = lly.getClassName();

      var b = tCls.split('___')[0];
      var lName = tCls.split('___')[1];
      if (b == 'b') {
        if (lName == baseMap) {
          lly.setVisible(true);
          this.SetCurrentBaseMap(baseMap);
        } else {
          lly.setVisible(false);
        }
      }
    }

    var extent: any;
    var XCoord = valStatus[1].split('=')[1];
    var YCoord = valStatus[2].split('=')[1];
    var zoo = valStatus[0].split('=')[1];
    var wktt = 'POINT (' + XCoord + ' ' + YCoord + ')';
    const format = new WKT();
    const feature = format.readFeature(wktt, {
      dataProjection: 'EPSG:3857', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    extent = feature.getGeometry()?.getExtent();
    this.mmap.getView().fit(extent);
    this.mmap.getView().setZoom(zoo);
  }

  drawWKTByLayerObject(data: any, lyrObj: any, vdNum: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#ff0010',
          width: 10,
        }),
      })
    );
    feature.setProperties({ vdNumber: vdNum });
    extent = feature.getGeometry()?.getExtent();
    featureArray.push(feature);
    lyrObj.getSource().addFeatures(featureArray);
  }

  drawWKTByLayerObjectForOnlySelect(data: any, lyrObj: any, vdNum: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#00ff87',
          width: 10,
        }),
      })
    );
    feature.setProperties({ vdNumber: vdNum });
    extent = feature.getGeometry()?.getExtent();
    featureArray.push(feature);
    lyrObj.getSource().addFeatures(featureArray);
  }

  draSplitedWKTByLayerObject(data: any, lyrObj: any, vdNum: any) {
    var extent: any;
    let featureArray: any = [];
    var that = this;

    const format = new WKT();
    var LPIWKT = data.split(' 0.0').join(''); //data[0].WKT.replace(' 0.0', '');;
    const feature = format.readFeature(LPIWKT, {
      dataProjection: 'EPSG:4326', //this.SourceProjection,
      featureProjection: 'EPSG:3857', //this.DestinationProjection,
    });

    var textStyle = new Text({
      text: vdNum.toString(),
      textAlign: 'center',
      textBaseline: 'middle',
      font: '15px Arial',
      fill: new Fill({
        color: '#ffffff',
      }),
      backgroundFill: new Fill({
        color: '#0015ff', // Specify the color for the background
      }),
      padding: [1, 1, 1, 1],
    });
    feature.setStyle(
      new Style({
        stroke: new Strok({
          color: '#0015ff',
          width: 10,
        }),
        text: textStyle,
      })
    );
    feature.setProperties({ vdNumber: vdNum });
    extent = feature.getGeometry()?.getExtent();
    featureArray.push(feature);
    lyrObj.getSource().addFeatures(featureArray);
  }

  ClearAllMemoryGeometryByLayerObject(lyrObj: any) {
    lyrObj.getSource().clear();
  }

  zoomToPolygonByLayerObject(lyrObj: any) {
    var extent = lyrObj.getSource().getExtent();
    this.mmap.getView().fit(extent, this.mmap.getSize());

    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    this.mmap.getView().setCenter(cen);
    this.mmap.getView().setZoom(zoo);
  }
}
