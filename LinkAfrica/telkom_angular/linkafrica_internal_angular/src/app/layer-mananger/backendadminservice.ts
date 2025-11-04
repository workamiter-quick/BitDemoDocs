import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ConfigManager } from '../config-ang';
import { GroupBeans, WmsBeans } from './beansobject';


@Injectable({
  providedIn: 'root',
})
export class AdminService {
  constructor(private http: HttpClient, private configmanager: ConfigManager) {
    this.groupBeansInput = new GroupBeans();

    // this.getAllGroupName();
    // this.getAllWmsLayers();
  }

  serviceBaseURL = this.configmanager.serviceBaseURL;
  groupBeansInput: any;


  getAllGroupName() {
    let url = this.serviceBaseURL + 'LM/GetGroup/0';
    return this.http.get(url);
  }

  saveGroup(GND: any) {
    this.groupBeansInput.GroupName = GND;
    this.groupBeansInput.UserID = '0';
    let url = this.serviceBaseURL + 'LM';
    return this.http.post(url, this.groupBeansInput);
  }

  getAllWmsLayers() {
    let url = this.serviceBaseURL + 'LM/GetAllWMS/0';
    return this.http.get(url);
  }
}
