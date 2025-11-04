export class IdentifyInputBeans {
    public XCoord: any;
    public YCoord: any;
    lstVisibleLayer: string[] = [];
}


export class AddressSearchBean {
  public Address: any;
}

export class GoogleCoordBean
{
    public Longitude_X:  any;
    public Latitude_Y: any;
    public Bandwidth:  any;
    public ContractTerm: any;
    public Longitude_X_Green:  any;
    public Latitude_Y_Green: any;
    public DDMaxDistance: any;
    public UseMaxDistance: any;
}

export class RoutInputCoord
{
    public StartLng_X:  any;
    public StartLat_Y: any;
    public EndLng_X:  any;
    public EndLat_Y: any;
    public Route_Mode: any;
}

export class CaptureDataBeans {
    public GID: any;
    public LPI: any;
    public LAYER: any;
    public WKT: any;
}


export class VDBeans {
    public VDData: any;
    public SelectedWardID: any;
}

export class SpatialBeansForSplit {
  public LineWKT: any;
  public PolygonWKT: any;
}

export class SplitedData {
  public DataOutput: any;
}


export class WardDetail {
    public TotalRegPop: string;
    public WardSize: string;
    public WardMin: string;
    public WardMax: string;
    public WardNumber: string;
  
    constructor(
      totalRegPop: string,
      wardSize: string,
      wardMin: string,
      wardMax: string,
      wardNumber: string
    ) {
      this.TotalRegPop = totalRegPop;
      this.WardSize = wardSize;
      this.WardMin = wardMin;
      this.WardMax = wardMax;
      this.WardNumber = wardNumber;
    }
  }