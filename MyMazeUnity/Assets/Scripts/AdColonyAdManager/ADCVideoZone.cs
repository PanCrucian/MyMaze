using UnityEngine;
using System.Collections;

public class ADCVideoZone {

  public string zoneId = "";
  public ADCVideoZoneType zoneType = ADCVideoZoneType.None;

  public ADCVideoZone(string newZoneId, ADCVideoZoneType newVideoZoneType) {
    zoneId = newZoneId;
    zoneType = newVideoZoneType;
	}
}
