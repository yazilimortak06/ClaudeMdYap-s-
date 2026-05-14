// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\chargeDevice\chargeDevice-list-model.ts
import { ChargeDeviceInstantState } from '../../enums/chargeDevice/charge-device-instant-state-enum';
import { ChargeDeviceOcppState } from '../../enums/chargeDevice/charge-device-ocpp-state-enum';
import { ChargeDevicePowerType, ChargeDeviceState } from '../../enums/chargeDevice/chargeDevice-type-enum';

export class ChargeDeviceListModel {
    id: number;
    name: string;
    serialNumber: string;
    stationId: number;
    stationName: string;
    companyId: number;
    companyName: string;
    state: ChargeDeviceState;
    powerType: ChargeDevicePowerType;
    instantState: ChargeDeviceInstantState;
    ocppState: ChargeDeviceOcppState;
    isConnected: boolean;
}
