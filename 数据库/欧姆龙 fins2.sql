-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        10.4.12-MariaDB - mariadb.org binary distribution
-- 服务器OS:                        Win64
-- HeidiSQL 版本:                  10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for yh_slix
CREATE DATABASE IF NOT EXISTS `yh_slix` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `yh_slix`;

-- Dumping structure for table yh_slix.alarm_level
CREATE TABLE IF NOT EXISTS `alarm_level` (
  `level_name` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  UNIQUE KEY `level_name` (`level_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报警等级';

-- Dumping data for table yh_slix.alarm_level: ~4 rows (大约)
/*!40000 ALTER TABLE `alarm_level` DISABLE KEYS */;
INSERT INTO `alarm_level` (`level_name`, `remark`) VALUES
	('A', '需要ME工程师处理'),
	('B', '需要现场操作员处理'),
	('C', '设备动作过程中的联锁及提示类信息'),
	('D', '备用');
/*!40000 ALTER TABLE `alarm_level` ENABLE KEYS */;

-- Dumping structure for table yh_slix.alarm_record
CREATE TABLE IF NOT EXISTS `alarm_record` (
  `did` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备编号',
  `UploadParamID` varchar(50) NOT NULL COMMENT '报警ID',
  `alarm_time` datetime NOT NULL COMMENT '报警发生时间',
  `dispose_time` datetime NOT NULL COMMENT '报警处理时间',
  `create_by` varchar(50) DEFAULT NULL COMMENT '创建人',
  `mhandler` varchar(50) DEFAULT NULL COMMENT '处理人',
  `duration` float(11,2) NOT NULL COMMENT '报警时长(min)',
  PRIMARY KEY (`did`),
  KEY `rule_did` (`EquipmentID`),
  KEY `alarm_time` (`alarm_time`),
  KEY `dispose_time` (`dispose_time`),
  KEY `UploadParamID` (`UploadParamID`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8 COMMENT='报警记录';

-- Dumping data for table yh_slix.alarm_record: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_record` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_record` ENABLE KEYS */;

-- Dumping structure for table yh_slix.alarm_temp
CREATE TABLE IF NOT EXISTS `alarm_temp` (
  `alarm_temp_did` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(20) NOT NULL COMMENT '规则编号',
  `UploadParamID` varchar(20) NOT NULL,
  `alarm_time` datetime NOT NULL DEFAULT current_timestamp(),
  `dispose_state` int(11) NOT NULL DEFAULT 0 COMMENT '0表示未处理，1表示已处理',
  `dispose_time` datetime DEFAULT NULL COMMENT '处理时间',
  `create_by` varchar(50) DEFAULT NULL COMMENT '创建人',
  `mhandler` varchar(50) DEFAULT NULL COMMENT '处理人',
  `duration` float(11,2) NOT NULL DEFAULT 0.00 COMMENT '报警时长(min)',
  PRIMARY KEY (`alarm_temp_did`),
  KEY `alarm_time` (`alarm_time`),
  KEY `dispose_state` (`dispose_state`),
  KEY `dispose_time` (`dispose_time`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8 COMMENT='dispose_state = 0 表示暂未处理的报警\r\ndispose_state = 1 表示暂已处理的报警\r\nduration表示报警时长(min)';

-- Dumping data for table yh_slix.alarm_temp: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_temp` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_temp` ENABLE KEYS */;

-- Dumping structure for table yh_slix.deivce_equipmentid_plc
CREATE TABLE IF NOT EXISTS `deivce_equipmentid_plc` (
  `EquipmentID` varchar(50) NOT NULL,
  `plcID` int(11) DEFAULT NULL,
  `A007Jason` mediumtext DEFAULT NULL,
  `ControlCodeAddress` varchar(50) DEFAULT NULL COMMENT 'MES控机地址',
  `ParentEQStateAddress` varchar(50) DEFAULT NULL COMMENT '主设备状态代码地址',
  `AndonStateAddress` varchar(50) DEFAULT NULL COMMENT 'andon状态地址',
  `QuantityAddress` varchar(50) DEFAULT NULL COMMENT '当班产量地址',
  `MESdisconnectedAddress` varchar(50) DEFAULT NULL COMMENT 'MES连接失败需要写入的地址',
  `HeatBeatAddress` varchar(50) DEFAULT NULL COMMENT '上位机写入心跳的地址',
  `MesReplyAddress` varchar(50) DEFAULT NULL COMMENT '写入MES回复信息超时的地址',
  `HmiPermissionRequestAddress` varchar(50) DEFAULT NULL COMMENT 'PLC变量；PLC请求上位机读取帐号密码,等待MES下发权限''',
  `AccountAddress` varchar(50) DEFAULT NULL COMMENT 'PLC变量保存了PLC里的帐号的寄存器地址',
  `CodeAddress` varchar(50) DEFAULT NULL COMMENT 'PLC变量保存了PLC里的密码的寄存器地址',
  `UserLevelAddress` varchar(50) DEFAULT NULL COMMENT 'MES下发给PLC的权限',
  `CountAddress` varchar(50) DEFAULT NULL COMMENT 'A007下发的count信息保存的地址',
  `ProductModeAddress` varchar(50) DEFAULT NULL COMMENT '0为量产，1为首件。只写一次',
  `SpartLimitControl` varchar(50) DEFAULT NULL COMMENT 'INT16寄存器地址',
  `InputLimitControl` varchar(50) DEFAULT NULL COMMENT 'INT16寄存器地址或 者西门子PLC的位地址',
  `ModelAddress` varchar(50) DEFAULT NULL COMMENT 'A007下发的Model数据对应的PLC地址或者为Andon文件夹路径',
  `SoftVersionAddress` varchar(50) DEFAULT NULL COMMENT '软件版本号对应的PLC地址',
  PRIMARY KEY (`EquipmentID`),
  KEY `FK_deivce_EquipmentID_plc_plc_config` (`plcID`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable` (`ControlCodeAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_2` (`ParentEQStateAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_3` (`AndonStateAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_4` (`MESdisconnectedAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_5` (`HeatBeatAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_6` (`MesReplyAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_7` (`HmiPermissionRequestAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_8` (`AccountAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_9` (`CodeAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_10` (`UserLevelAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_11` (`QuantityAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_12` (`CountAddress`),
  KEY `FK_deivce_equipmentid_plc_user_define_variable_13` (`ProductModeAddress`),
  CONSTRAINT `FK_deivce_EquipmentID_plc_plc_config` FOREIGN KEY (`plcID`) REFERENCES `plc_config` (`plc_ID`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable` FOREIGN KEY (`ControlCodeAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_10` FOREIGN KEY (`UserLevelAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_11` FOREIGN KEY (`QuantityAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_12` FOREIGN KEY (`CountAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_13` FOREIGN KEY (`ProductModeAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_2` FOREIGN KEY (`ParentEQStateAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_3` FOREIGN KEY (`AndonStateAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_4` FOREIGN KEY (`MESdisconnectedAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_5` FOREIGN KEY (`HeatBeatAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_6` FOREIGN KEY (`MesReplyAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_7` FOREIGN KEY (`HmiPermissionRequestAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_8` FOREIGN KEY (`AccountAddress`) REFERENCES `user_define_variable` (`variableName`),
  CONSTRAINT `FK_deivce_equipmentid_plc_user_define_variable_9` FOREIGN KEY (`CodeAddress`) REFERENCES `user_define_variable` (`variableName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='EquipmentID与PLC关系表\r\n对于板卡设备，plcID为Null\r\n一个Baking设备，有多个PLC，没有ChildEQ,一个PLC对应多个EquipmentID\r\nDegassing、注液机设备，只有一个PLC，多个ChildEQ\r\n其他PLC设备，都是只有一个EquipmentID，无ChildEQ，PLC数量可能多个\r\nControlCodeAddress用于A007或者A011接口MES指令控制设备，对于PLC设备，该值为寄存器的地址。参照device_controlcode表来写数字到PLC里面。\r\nParentEQStateAddress用于A019接口上传主设备状态代码。ParentEQStateAddress表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nAndonStateAddress用于A019接口上传Andon状态代码。AndonStateAddress表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nQuantityAddress用于A019接口上传当班产量。QuantityAddress表示寄存器地址，或者系统变量名。读取到PLC值后，上传。\r\nHeatBeatAddress只针对PLC设备\r\nMesReplyAddress用来写入MES超时与否的信息\r\ndevice_controlcode表里配置了：\r\nHeatBeat\r\nMESconnected\r\nMESdisconnected\r\nMesReplyNG\r\nMesReplyOK\r\ndevice_controlcode \r\n对应的PLC值的信息\r\nUserLevelAddress 里写的值206、307、408分别表示操作、维修、开发三级权限\r\nStateCodeAddress对应的StateCode等于1表示做首件，否则等于0；CountAddress对应的Count表示首件数量\r\n\r\n';

-- Dumping data for table yh_slix.deivce_equipmentid_plc: ~1 rows (大约)
/*!40000 ALTER TABLE `deivce_equipmentid_plc` DISABLE KEYS */;
INSERT INTO `deivce_equipmentid_plc` (`EquipmentID`, `plcID`, `A007Jason`, `ControlCodeAddress`, `ParentEQStateAddress`, `AndonStateAddress`, `QuantityAddress`, `MESdisconnectedAddress`, `HeatBeatAddress`, `MesReplyAddress`, `HmiPermissionRequestAddress`, `AccountAddress`, `CodeAddress`, `UserLevelAddress`, `CountAddress`, `ProductModeAddress`, `SpartLimitControl`, `InputLimitControl`, `ModelAddress`, `SoftVersionAddress`) VALUES
	('SLIX011U', 1, '{"Header":{"SessionID":"78359ab3-c82f-4469-95e8-9524904aede1","FunctionID":"A007","PCName":"PCName","EQCode":"SLIX011U","SoftName":"ServerSoft","RequestTime":"2019-12-09 02:07:18 607"},"RequestInfo":{"Count":"50","CmdInfo":{"ControlCode":"Run","StateCode":"1","StateDesc":""},"UserInfo":{"UserID":"123","UserName":"ATL","UserLevel":"1"},"ResourceInfo":{"ResourceID":"EQ00000001","ResourceShift":"M"},"SpartInfo":[{"SpartName":"边角料压轮","SpartID":"000000070000275426","SpartExpectedLifetime":"169970.50","ChangeFlag":"false"},{"SpartName":"边角料压轮","SpartID":"000000070000275427","SpartExpectedLifetime":"169970.50","ChangeFlag":"false"},{"SpartName":"测长编码轮","SpartID":"000000070000275425","SpartExpectedLifetime":"47","ChangeFlag":"false"},{"SpartName":"毛刷","SpartID":"000000070000275423","SpartExpectedLifetime":"169970.50","ChangeFlag":"false"},{"SpartName":"毛刷","SpartID":"000000070000275424","SpartExpectedLifetime":"47","ChangeFlag":"false"}],"ModelInfo":"SA-ACE-486690-010L","ParameterInfo":[{"ParamID":"16214","StandardValue":"10.000","UpperLimitValue":"50","LowerLimitValue":"10","Description":"分条速度 （m/min）"}]}}', 'ControlCode', 'ParentEQState', 'AndonState', 'Quantity', 'MESstatusToPLC', 'HeatBeat', 'MesReply', 'HmiPermissionRequest', 'Account', 'Code', 'UserLevel', 'A007Count', 'StateCode', NULL, NULL, NULL, NULL);
/*!40000 ALTER TABLE `deivce_equipmentid_plc` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_alert_config
CREATE TABLE IF NOT EXISTS `device_alert_config` (
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `plcID` int(11) DEFAULT NULL,
  `UploadParamID` varchar(50) NOT NULL COMMENT '报警编码',
  `AlertLevel` varchar(20) NOT NULL COMMENT '报警等级',
  `AlertBitAddr` varchar(50) DEFAULT NULL COMMENT '报警地址',
  `ParamName` varchar(50) NOT NULL COMMENT '报警信息',
  PRIMARY KEY (`EquipmentID`,`UploadParamID`),
  KEY `FK_device_alert_config_alarm_level` (`AlertLevel`),
  CONSTRAINT `FK_device_alert_config_alarm_level` FOREIGN KEY (`AlertLevel`) REFERENCES `alarm_level` (`level_name`),
  CONSTRAINT `FK_device_alert_config_deivce_equipmentid_plc` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid_plc` (`EquipmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='设备报警配置\r\nisAlarming字段适用于板卡设备，报警信息并非从PLC中获取得到。对于板卡设备，AlertBitAddr字段无意义。';

-- Dumping data for table yh_slix.device_alert_config: ~88 rows (大约)
/*!40000 ALTER TABLE `device_alert_config` DISABLE KEYS */;
INSERT INTO `device_alert_config` (`EquipmentID`, `plcID`, `UploadParamID`, `AlertLevel`, `AlertBitAddr`, `ParamName`) VALUES
	('SLIX011U', 1, '1611000', 'B', 'D16110.0', '急停故障'),
	('SLIX011U', 1, '1611001', 'A', 'D16110.1', '放卷伺服故障'),
	('SLIX011U', 1, '1611002', 'A', 'D16110.2', '主传动伺服故障'),
	('SLIX011U', 1, '1611003', 'A', 'D16110.3', '切刀伺服故障'),
	('SLIX011U', 1, '1611004', 'A', 'D16110.4', '上出刀辊伺服故障'),
	('SLIX011U', 1, '1611005', 'A', 'D16110.5', '下出刀辊伺服故障'),
	('SLIX011U', 1, '1611006', 'A', 'D16110.6', '左边角料伺服故障'),
	('SLIX011U', 1, '1611007', 'A', 'D16110.7', '右边角料伺服故障'),
	('SLIX011U', 1, '1611008', 'A', 'D16110.8', '上收卷隔断伺服故障'),
	('SLIX011U', 1, '1611009', 'A', 'D16110.9', '下收卷隔断伺服故障'),
	('SLIX011U', 1, '1611010', 'A', 'D16110.10', '上穿带伺服故障'),
	('SLIX011U', 1, '1611011', 'A', 'D16110.11', '下穿带伺服故障'),
	('SLIX011U', 1, '1611012', 'A', 'D16110.12', '上收卷伺服故障'),
	('SLIX011U', 1, '1611013', 'A', 'D16110.13', '下收卷伺服故障'),
	('SLIX011U', 1, '1611014', 'A', 'D16110.14', '上边料伺服故障'),
	('SLIX011U', 1, '1611015', 'A', 'D16110.15', '下边料伺服故障'),
	('SLIX011U', 1, '1611100', 'C', 'D16111.0', '门控打开故障'),
	('SLIX011U', 1, '1611101', 'A', 'D16111.1', '上出刀辊上升限位故障'),
	('SLIX011U', 1, '1611102', 'A', 'D16111.2', '上出刀辊下降限位故障'),
	('SLIX011U', 1, '1611103', 'A', 'D16111.3', '下出刀辊上升限位故障'),
	('SLIX011U', 1, '1611104', 'A', 'D16111.4', '下出刀辊下降限位故障'),
	('SLIX011U', 1, '1611105', 'B', 'D16111.5', '上跟踪辊后退限位故障'),
	('SLIX011U', 1, '1611106', 'B', 'D16111.6', '下跟踪辊后退限位故障'),
	('SLIX011U', 1, '1611107', 'B', 'D16111.7', '上抚平辊上限限位故障'),
	('SLIX011U', 1, '1611108', 'B', 'D16111.8', '下抚平辊上限限位故障'),
	('SLIX011U', 1, '1611109', 'B', 'D16111.9', '放卷断带故障'),
	('SLIX011U', 1, '1611110', 'B', 'D16111.10', '上滑差轴没有充气故障'),
	('SLIX011U', 1, '1611111', 'B', 'D16111.11', '下滑差轴没有充气故障'),
	('SLIX011U', 1, '1611112', 'B', 'D16111.12', '上滑差轴没有固定故障'),
	('SLIX011U', 1, '1611113', 'B', 'D16111.13', '下滑差轴没有固定故障'),
	('SLIX011U', 1, '1611114', 'B', 'D16111.14', '刀架没有安装到位故障'),
	('SLIX011U', 1, '1611115', 'C', 'D16111.15', '光幕被挡到故障'),
	('SLIX011U', 1, '1611200', 'C', 'D16112.0', '切刀寿命到达故障'),
	('SLIX011U', 1, '1611201', 'C', 'D16112.1', '分切长度到达设定值故障'),
	('SLIX011U', 1, '1611202', 'B', 'D16112.2', '气压过低故障'),
	('SLIX011U', 1, '1611203', 'B', 'D16112.3', '除尘真空压力不足故障'),
	('SLIX011U', 1, '1611204', 'B', 'D16112.4', '极片长度异常停机故障'),
	('SLIX011U', 1, '1611205', 'B', 'D16112.5', '放卷张力未打开故障'),
	('SLIX011U', 1, '1611206', 'B', 'D16112.6', '上跟踪辊不在自动状态故障'),
	('SLIX011U', 1, '1611207', 'B', 'D16112.7', '下跟踪辊不在自动状态故障'),
	('SLIX011U', 1, '1611208', 'B', 'D16112.8', '上抚平辊不在自动状态故障'),
	('SLIX011U', 1, '1611209', 'B', 'D16112.9', '下抚平辊不在自动状态故障'),
	('SLIX011U', 1, '1611210', 'B', 'D16112.10', '极片压带没有抬起故障'),
	('SLIX011U', 1, '1611211', 'B', 'D16112.11', '空箔压带没有抬起故障'),
	('SLIX011U', 1, '1611212', 'B', 'D16112.12', '左蓝标感应故障'),
	('SLIX011U', 1, '1611213', 'B', 'D16112.13', '右蓝标感应故障'),
	('SLIX011U', 1, '1611214', 'A', 'D16112.14', 'ANDON系统未启动故障'),
	('SLIX011U', 1, '1611215', 'B', 'D16112.15', '超时未选择停机原因故障'),
	('SLIX011U', 1, '1611301', 'B', 'D16113.1', '穿带正在运行中故障'),
	('SLIX011U', 1, '1611302', 'B', 'D16113.2', '检测片数达到设定值故障'),
	('SLIX011U', 1, '1611303', 'C', 'D16113.3', '刀模预警米数达到故障'),
	('SLIX011U', 1, '1611304', 'C', 'D16113.4', '刀模寿命米数达到故障'),
	('SLIX011U', 1, '1611305', 'B', 'D16113.5', '分切宽度与刀模系统不符故障'),
	('SLIX011U', 1, '1611306', 'B', 'D16113.6', '刀架编号读取超时故障'),
	('SLIX011U', 1, '1611308', 'B', 'D16113.8', '停机原因未选择禁止开机'),
	('SLIX011U', 1, '1611309', 'A', 'D16113.9', 'ANDON工单未处理报警'),
	('SLIX011U', 1, '1611310', 'B', 'D16113.10', '酒精泵酒精消耗完故障'),
	('SLIX011U', 1, '1611311', 'C', 'D16113.11', 'CCD未准备好禁止开机'),
	('SLIX011U', 1, '1611312', 'B', 'D16113.12', 'CCD停机脉冲故障'),
	('SLIX011U', 1, '1611313', 'A', 'D16113.13', 'PLC_UP通讯错误'),
	('SLIX011U', 1, '1611314', 'A', 'D16113.14', 'ANDON系统通讯异常'),
	('SLIX011U', 1, '1611500', 'B', 'D16115.0', '纠偏限位警告'),
	('SLIX011U', 1, '1611501', 'C', 'D16115.1', '左边角料断带警告'),
	('SLIX011U', 1, '1611502', 'C', 'D16115.2', '右边角料断带警告'),
	('SLIX011U', 1, '1611503', 'C', 'D16115.3', '上边料断带警告'),
	('SLIX011U', 1, '1611504', 'C', 'D16115.4', '下边料断带警告'),
	('SLIX011U', 1, '1611505', 'C', 'D16115.5', '放卷卷径预警'),
	('SLIX011U', 1, '1611506', 'C', 'D16115.6', '收卷卷径预警'),
	('SLIX011U', 1, '1611507', 'C', 'D16115.7', '耗材预警'),
	('SLIX011U', 1, '1611508', 'C', 'D16115.8', '耗材寿命到期'),
	('SLIX011U', 1, '1611509', 'C', 'D16115.9', '刀模寿命系统与机台没有联动警告'),
	('SLIX011U', 1, '1611510', 'C', 'D16115.10', '放卷气涨轴未固定好警告'),
	('SLIX011U', 1, '1611512', 'B', 'D16115.12', 'B面涂长长度异常'),
	('SLIX011U', 1, '1611513', 'B', 'D16115.13', 'B面留白长度异常'),
	('SLIX011U', 1, '1611514', 'B', 'D16115.14', 'A面涂长长度异常'),
	('SLIX011U', 1, '1611515', 'B', 'D16115.15', 'A面留白长度异常'),
	('SLIX011U', 1, '1611600', 'B', 'D16116.0', '前错位长度异常'),
	('SLIX011U', 1, '1611601', 'B', 'D16116.1', '后错位长度异常'),
	('SLIX011U', 1, '1611602', 'B', 'D16116.2', '极片长度异常警告'),
	('SLIX011U', 1, '1611603', 'C', 'D16116.3', '上抚平辊没有压下警告'),
	('SLIX011U', 1, '1611604', 'C', 'D16116.4', '下抚平辊没有压下警告'),
	('SLIX011U', 1, '1611605', 'B', 'D16116.5', '不良片异常停机'),
	('SLIX011U', 1, '1611606', 'C', 'D16116.6', '边角料箱门没关好警告'),
	('SLIX011U', 1, '1611607', 'C', 'D16116.7', 'CCD打标机未抬起不能穿带'),
	('SLIX011U', 1, '1611608', 'C', 'D16116.8', '停机时间过长建议断电'),
	('SLIX011U', 1, '1611609', 'C', 'D16116.9', '放卷卷径过小'),
	('SLIX011U', 1, '1611610', 'C', 'D16116.10', '收卷卷径过大'),
	('SLIX011U', 1, '1611612', 'C', 'D16116.12', '毛毡未压下警告');
/*!40000 ALTER TABLE `device_alert_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_childeqcode
CREATE TABLE IF NOT EXISTS `device_childeqcode` (
  `ChildEQCode` varchar(50) NOT NULL COMMENT '填写内容：工位code',
  `ChildEQStateAddress` varchar(50) DEFAULT NULL,
  `Remark` varchar(50) NOT NULL,
  PRIMARY KEY (`ChildEQCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='一个Baking设备，有多个PLC，没有ChildEQ,一个PLC对应多个EquipmentID\r\nDegassing、注液机设备，只有一个PLC，多个ChildEQ\r\n其他PLC设备，都是只有一个EquipmentID，无ChildEQ，PLC数量可能多个\r\nChildEQStateAddress可以为系统变量，或者PLC变量，但是必须处于R类型的地址段\r\nChildEQState为enable或者disable\r\n';

-- Dumping data for table yh_slix.device_childeqcode: ~0 rows (大约)
/*!40000 ALTER TABLE `device_childeqcode` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_childeqcode` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_controlcode
CREATE TABLE IF NOT EXISTS `device_controlcode` (
  `ControlCode` varchar(50) NOT NULL,
  `StateCode` varchar(50) NOT NULL,
  `describe` varchar(200) NOT NULL,
  `plcValue` int(11) NOT NULL,
  PRIMARY KEY (`ControlCode`,`StateCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='A007或者A011\r\nMES控机指令表\r\nplcValue：要写入的值';

-- Dumping data for table yh_slix.device_controlcode: ~29 rows (大约)
/*!40000 ALTER TABLE `device_controlcode` DISABLE KEYS */;
INSERT INTO `device_controlcode` (`ControlCode`, `StateCode`, `describe`, `plcValue`) VALUES
	('DOWN', 'DOWN', '设备通用宕机', 10),
	('DOWN', 'FAC_DOWN', '由于厂务系统问题而把设备挂DOWN 机', 13),
	('DOWN', 'IE Schedule Down', '设备正常，但是考虑产能闲置的原因，需要把设备从产线拉下来投入到仓库', 15),
	('DOWN', 'Material_DOWN', '由于来料问题而把设备挂DOWN 机', 14),
	('DOWN', 'MES_DOWN', '由于MES 系统出现问题而ba设备挂DOWN 机', 12),
	('DOWN', 'Move_Location', '设备正常，设备根据IE 要求调换拉线或区域', 17),
	('DOWN', 'PM_DOWN', '由于PM 而造成的宕机', 11),
	('DOWN', 'Shift_Down', '设备正常，由于设备午休时停机', 16),
	('HeatBeat', '', '写入PLC的心跳值', 10),
	('HOLD_ENG(Suspend)', 'HOLD', '由MES 下达指令给到设备，设备暂停加工，不允许新的物料进入设备中加工', 8),
	('HOLD_ENG(Suspend)', 'WAIT_ENG', '生产部发现设备有问题，将设备交由工程师处理', 9),
	('IDLE', 'IDLE', '设备空闲', 3),
	('IDLE', 'Switch_Shift', '设备正常，由于需要换班或午休', 5),
	('IDLE', 'WAIT_Material', '设备正常，因为无料生产等候', 4),
	('IDLE', 'WAIT_PRD', '设备工程师修完机之后交付给生产部', 6),
	('InputLimitControl', 'SET', 'Input总控，下发Input成功后输出总控开关值', 1),
	('MESconnected', '', 'MES连接成功', 10),
	('MESdisconnected', '', 'MES连接失败', 20),
	('MesReplyNG', '', '当3次未回复指令为A015、A013、A035、A055、A033、A029、A031、A051时需要向设备发出停机指令，并启动设备的有声报警信号提示MES发生异常需要介入处理', 0),
	('MesReplyOK', '', '非MesReplyNG状态', 0),
	('PM', '', '设备做PM', 7),
	('RUN', '', '设备正在正常加工产品', 20),
	('SingleInputLimitControl', 'Disable', 'Input单控，MES不需要下发Input的开关值', 10),
	('SingleInputLimitControl', 'Enable', 'Input单控，MES需要下发Input的开关值', 0),
	('SingleSpartLimitControl', 'Disable', '易损件单控，不需要下发易损件开关值', 0),
	('SingleSpartLimitControl', 'Enable', '易损件单控，需要下发易损件开关值', 1),
	('SpartLimitControl', 'SET', '易损件总控，下发易损件信息成功后输出总控开关值', 1),
	('STOP', '', 'MES要求停机', 10),
	('TEST', '', ' 测机、首件加工时设置MES 为Test；要保证完成首件任务后不能投入批量生产', 2);
/*!40000 ALTER TABLE `device_controlcode` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_inputoutput_config
CREATE TABLE IF NOT EXISTS `device_inputoutput_config` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `plcID` int(11) DEFAULT NULL,
  `SendParamID` varchar(50) NOT NULL COMMENT 'input参数ID-A007',
  `UploadParamID` varchar(50) NOT NULL COMMENT 'output参数ID-A013、监控地址A045、A017点检',
  `ParamName` varchar(50) NOT NULL COMMENT '参数名称',
  `Type` varchar(20) NOT NULL COMMENT '参数类型 float',
  `SettingValueAddr` varchar(50) NOT NULL COMMENT '设定值地址-A007',
  `UpperLimitValueAddr` varchar(50) NOT NULL COMMENT '设定上限地址-A007',
  `LowerLimitValueAddr` varchar(50) NOT NULL COMMENT '设定下限地址-A007',
  `LimitControl` varchar(50) NOT NULL COMMENT '参数是否启用控制地址',
  `InputChangeMonitorAddr` varchar(50) NOT NULL COMMENT '设定参数变更监控地址-A045；Input参数下发界面的当前值监控地址',
  `ActualValueAddr` varchar(50) NOT NULL COMMENT '实际值地址-A017点检',
  `BycellOutputAddr` varchar(50) NOT NULL COMMENT 'by产品的过程值地址-A013',
  `ParamValueRatio` float NOT NULL COMMENT '参数倍率',
  PRIMARY KEY (`ID`),
  KEY `EquipmentID` (`EquipmentID`),
  KEY `SendParamID` (`SendParamID`),
  KEY `UploadParamID` (`UploadParamID`),
  KEY `FK_value_types` (`Type`),
  CONSTRAINT `FK_device_in_out_put_mes_config_deivce_equipmentid_plc` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid_plc` (`EquipmentID`),
  CONSTRAINT `FK_value_types` FOREIGN KEY (`Type`) REFERENCES `valuetypes` (`valueType`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8 COMMENT='从A060接口接收到的Input参数和Output参数地址配置信息\r\n\r\nEnableAddr对应的自定义变量用于表示是否启用该Input参数，该自定义变量的值等于10表示不启用，等于0表示启用。此表可能配置了100个参数，\r\n但是A007接口可能只下发了几个。A007下发的那几个参数对应的EnableAddr等于0，其他的等于10.\r\n\r\n当同一个参数的input参数ID和output参数ID不同时上传接收A007指令时使用input参数ID进行接收，上传A013、A017数据时使用output参数ID进行上传；\r\n当同一个参数的input参数ID和output参数ID不同时监控input参数设定值变化A045的上传指令使用output参数ID进行上传；\r\n\r\ndownloadStatus等于1表示下载完成.\r\nMES下发model后软件需要将downloadStatus字段值改为0.\r\nvariableTypeID与variableType表里的variableTypeID对应，默认为0，表示刚从MES下载此配置，暂未判断addr为PLC地址，还是系统变量名称';

-- Dumping data for table yh_slix.device_inputoutput_config: ~13 rows (大约)
/*!40000 ALTER TABLE `device_inputoutput_config` DISABLE KEYS */;
INSERT INTO `device_inputoutput_config` (`ID`, `EquipmentID`, `plcID`, `SendParamID`, `UploadParamID`, `ParamName`, `Type`, `SettingValueAddr`, `UpperLimitValueAddr`, `LowerLimitValueAddr`, `LimitControl`, `InputChangeMonitorAddr`, `ActualValueAddr`, `BycellOutputAddr`, `ParamValueRatio`) VALUES
	(1, 'SLIX011U', 1, '16214', '16214', '分切速度', 'Float', 'D18180', 'D18182', 'D18184', 'D17000', 'D16330', 'D16500', '', 1),
	(2, 'SLIX011U', 1, '2', '16209', '放卷单位张力', 'Float', 'D18186', 'D18188', 'D18190', 'D17000', 'D16336', 'D16502', '', 1),
	(3, 'SLIX011U', 1, '3', '50948', '上收卷单位张力', 'Float', 'D18192', 'D18194', 'D18196', 'D17000', 'D16342', 'D16504', '', 1),
	(4, 'SLIX011U', 1, '4', '50949', '下收卷单位张力', 'Float', 'D18198', 'D18200', 'D18202', 'D17000', 'D16348', 'D16506', '', 1),
	(5, 'SLIX011U', 1, '5', '50823', '测长上sensor设定值', 'Float', 'D18204', 'D18206', 'D18208', 'D17000', 'D16354', 'D16508', '', 1),
	(6, 'SLIX011U', 1, '6', '50824', '测长下sensor设定值', 'Float', 'D18210', 'D18212', 'D18214', 'D17000', 'D16360', 'D16510', '', 1),
	(7, 'SLIX011U', 1, '7', '50825', '收卷最大卷径', 'Float', 'D18216', 'D18218', 'D18220', 'D17000', 'D16366', 'D16512', '', 1),
	(8, 'SLIX011U', 1, '8', '50826', '上分切条数', 'Float', 'D18222', 'D18224', 'D18226', 'D17000', 'D16372', 'D16514', '', 1),
	(9, 'SLIX011U', 1, '9', '50827', '下分切条数', 'Float', 'D18228', 'D18230', 'D18232', 'D17000', 'D16378', 'D16516', '', 1),
	(10, 'SLIX011U', 1, '10', '50730', '上收卷实际卷径', 'Float', '', '', '', 'D17000', '', 'D16518', '', 1),
	(11, 'SLIX011U', 1, '11', '50731', '下收卷实际卷径', 'Float', '', '', '', 'D17000', '', 'D16520', '', 1),
	(12, 'SLIX011U', 1, '12', '50830', '分切总条数', 'Float', '', '', '', 'D17000', '', 'D16522', '', 1),
	(13, 'SLIX011U', 1, '13', '51230', '负压实际值', 'Float', '', '', '', 'D17000', '', 'D16524', '', 1);
/*!40000 ALTER TABLE `device_inputoutput_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_spart_config
CREATE TABLE IF NOT EXISTS `device_spart_config` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `plcID` int(11) DEFAULT NULL,
  `UploadParamID` varchar(50) NOT NULL COMMENT '关键配件ID',
  `ParamName` varchar(50) NOT NULL COMMENT '关键配件名称',
  `Type` varchar(20) NOT NULL COMMENT '参数类型 Float,Int32',
  `SpartExpectedLifetime` int(11) NOT NULL COMMENT '关键配件预期寿命-A007下发值',
  `MesSettingUsedLife` float NOT NULL COMMENT 'MES下发当前使用寿命',
  `NeedDownLoadSpartExpectedLifetimeToPLC` int(11) NOT NULL,
  `SettingValueAddr` varchar(50) NOT NULL COMMENT '关键配件预期寿命-A060下发地址',
  `SettingActualValueAddr` varchar(50) NOT NULL COMMENT 'MES下发的实际使用寿命地址',
  `ActualValueAddr` varchar(50) NOT NULL COMMENT '关键配件实际使用寿命-A021上传给MES',
  `LimitControl` varchar(50) DEFAULT NULL COMMENT '若是位地址，则必须是HSL支持的地址格式；除了是位地址以及倍福PLC地址，那就是Int16类型地址',
  `ParamValueRatio` float NOT NULL COMMENT '倍率',
  `PercentWarning` float(11,7) NOT NULL COMMENT '预警阈值',
  `ChangeDate` datetime NOT NULL DEFAULT current_timestamp(),
  `ChangeUser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `EquipmentID_UploadParamID` (`EquipmentID`,`UploadParamID`),
  KEY `FK` (`Type`),
  CONSTRAINT `FK` FOREIGN KEY (`Type`) REFERENCES `valuetypes` (`valueType`),
  CONSTRAINT `FK_device_spart_config_deivce_equipmentid_plc` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid_plc` (`EquipmentID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COMMENT='易损件配置表\r\n在更好零部件的时候，需要将本表的数据（被更换的零部件的数据）剪切放入历史表quickwearparthistoryinfo\r\nSettingValueAddr为PLC地址或者系统变量名\r\nActualValueAddr为PLC地址或者系统变量名\r\nSpartExpectedLifetime为MES A007接口下发的值，作为本地保存，不会自动保存到PLC中。需要额外下发到PLC或者系统变量\r\nNeedDownLoadSpartExpectedLifetimeToPLC = 1表示需要下载到PLC\r\n在PLC通信正常的情况下，在一个单独的线程里，预期寿命下载完成后，上位机对ActualValueAddr地址里的值清零。将自动将NeedDownLoadSpartExpectedLifetimeToPLC改为0';

-- Dumping data for table yh_slix.device_spart_config: ~5 rows (大约)
/*!40000 ALTER TABLE `device_spart_config` DISABLE KEYS */;
INSERT INTO `device_spart_config` (`id`, `EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `MesSettingUsedLife`, `NeedDownLoadSpartExpectedLifetimeToPLC`, `SettingValueAddr`, `SettingActualValueAddr`, `ActualValueAddr`, `LimitControl`, `ParamValueRatio`, `PercentWarning`, `ChangeDate`, `ChangeUser`) VALUES
	(1, 'SLIX011U', 1, 'SLIX-SMS0-0', '上毛刷', 'Float', 1001, 0, 0, 'D18100', 'D18100', 'D16150', '', 1, 0.9500000, '2019-11-28 14:23:39', 'ATL'),
	(2, 'SLIX011U', 1, 'SLIX-XMS0-0', '下毛刷', 'Float', 1015, 0, 0, 'D18102', 'D18102', 'D16152', '', 1, 0.9500000, '2019-11-28 14:23:39', 'ATL'),
	(3, 'SLIX011U', 1, 'SLIX-CCJL-0', '测长编码胶辊', 'Float', 102, 0, 0, 'D18110', 'D18110', 'D16160', '', 1, 0.9500000, '2019-11-28 14:23:39', 'ATL'),
	(4, 'SLIX011U', 1, 'SLIX-ZBJ0-0', '左边角料压辊', 'Float', 102, 0, 0, 'D18114', 'D18114', 'D16164', '', 1, 0.9500000, '2019-11-28 14:23:39', 'ATL'),
	(5, 'SLIX011U', 1, 'SLIX-YBJ0-0', '右边角料压辊', 'Float', 1022, 0, 0, 'D18116', 'D18116', 'D16166', '', 1, 0.9500000, '2019-11-28 14:23:39', 'ATL');
/*!40000 ALTER TABLE `device_spart_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_status_code
CREATE TABLE IF NOT EXISTS `device_status_code` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `key` varchar(50) NOT NULL,
  `status` varchar(50) NOT NULL COMMENT '状态',
  `plcValue` int(11) NOT NULL COMMENT '从PLC里读到的值',
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `key` (`key`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COMMENT='上位机上传PLC里寄存器里的状态值给MES';

-- Dumping data for table yh_slix.device_status_code: ~7 rows (大约)
/*!40000 ALTER TABLE `device_status_code` DISABLE KEYS */;
INSERT INTO `device_status_code` (`id`, `key`, `status`, `plcValue`, `remark`) VALUES
	(1, 'ParentEQStateCode', 'Run', 22, '上传当前设备状态：Run'),
	(2, 'ParentEQStateCode', 'Stop', 11, '上传当前设备状态：Stop'),
	(3, 'AndonState', 'Run', 11, '上传当前Andon状态：Run'),
	(4, 'AndonState', 'Stop', 0, '上传当前Andon状态：Stop'),
	(9, 'ChildEQState', 'Enable', 0, '当前子设备已启用'),
	(10, 'ChildEQState', 'Disable', 1, '当前子设备已停用'),
	(11, 'ParentEQStateCode', 'Alarm', 33, '上传当前设备状态：Alarm');
/*!40000 ALTER TABLE `device_status_code` ENABLE KEYS */;

-- Dumping structure for table yh_slix.device_tag
CREATE TABLE IF NOT EXISTS `device_tag` (
  `plcID` int(11) NOT NULL,
  `tagName` varchar(50) NOT NULL DEFAULT '',
  `valueType` varchar(50) NOT NULL DEFAULT '',
  `description` varchar(200) NOT NULL DEFAULT '',
  UNIQUE KEY `plcID_tagName` (`plcID`,`tagName`),
  KEY `plcID` (`plcID`),
  KEY `FK_device_tag_valuetypes` (`valueType`),
  CONSTRAINT `FK_device_tag_plc_config` FOREIGN KEY (`plcID`) REFERENCES `plc_config` (`plc_ID`),
  CONSTRAINT `FK_device_tag_valuetypes` FOREIGN KEY (`valueType`) REFERENCES `valuetypes` (`valueType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='倍福PLC、CIP协议PLC的设备标签表';

-- Dumping data for table yh_slix.device_tag: ~0 rows (大约)
/*!40000 ALTER TABLE `device_tag` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_tag` ENABLE KEYS */;

-- Dumping structure for table yh_slix.facility_info
CREATE TABLE IF NOT EXISTS `facility_info` (
  `facility_did` int(11) NOT NULL COMMENT '主键',
  `MMName` varchar(255) DEFAULT NULL COMMENT '设备/工序名称',
  `State` int(11) NOT NULL COMMENT '工位状态（0：正常、1：故障）上位机２秒更新1次',
  `StateAddress` varchar(50) DEFAULT NULL COMMENT '工位状态地址',
  `Remark` varchar(250) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`facility_did`),
  KEY `FK_facility_info_user_define_variable` (`StateAddress`),
  CONSTRAINT `FK_facility_info_user_define_variable` FOREIGN KEY (`StateAddress`) REFERENCES `user_define_variable` (`variableName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='工位';

-- Dumping data for table yh_slix.facility_info: ~0 rows (大约)
/*!40000 ALTER TABLE `facility_info` DISABLE KEYS */;
/*!40000 ALTER TABLE `facility_info` ENABLE KEYS */;

-- Dumping structure for table yh_slix.hmi_setting_monitor_log
CREATE TABLE IF NOT EXISTS `hmi_setting_monitor_log` (
  `logID` int(11) NOT NULL AUTO_INCREMENT,
  `variableID` int(11) DEFAULT 0 COMMENT '变量ID',
  `variableName` varchar(50) NOT NULL DEFAULT '' COMMENT '变量名',
  `address` varchar(50) DEFAULT NULL COMMENT 'PLC地址',
  `description` varchar(50) DEFAULT NULL,
  `plc_ID` int(11) DEFAULT NULL COMMENT '数据源',
  `variableTypeID` int(11) NOT NULL,
  `value` varchar(50) NOT NULL,
  `logDateTime` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`logID`),
  KEY `logDateTime` (`logDateTime`),
  KEY `FK_hmi_setting_monitor_log_plc_config` (`plc_ID`),
  KEY `FK_hmi_setting_monitor_log_variabletype` (`variableTypeID`),
  KEY `FK_hmi_setting_monitor_log_user_define_variable` (`variableID`),
  CONSTRAINT `FK_hmi_setting_monitor_log_plc_config` FOREIGN KEY (`plc_ID`) REFERENCES `plc_config` (`plc_ID`),
  CONSTRAINT `FK_hmi_setting_monitor_log_variabletype` FOREIGN KEY (`variableTypeID`) REFERENCES `variabletype` (`variableTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=52 DEFAULT CHARSET=utf8 COMMENT='用户自定义变量里重要参数记录，数据有变化的时候才记录';

-- Dumping data for table yh_slix.hmi_setting_monitor_log: ~0 rows (大约)
/*!40000 ALTER TABLE `hmi_setting_monitor_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `hmi_setting_monitor_log` ENABLE KEYS */;

-- Dumping structure for table yh_slix.input_log_from
CREATE TABLE IF NOT EXISTS `input_log_from` (
  `source` varchar(50) NOT NULL,
  PRIMARY KEY (`source`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='input参数来源';

-- Dumping data for table yh_slix.input_log_from: ~2 rows (大约)
/*!40000 ALTER TABLE `input_log_from` DISABLE KEYS */;
INSERT INTO `input_log_from` (`source`) VALUES
	('download'),
	('upload');
/*!40000 ALTER TABLE `input_log_from` ENABLE KEYS */;

-- Dumping structure for table yh_slix.input_variable_history
CREATE TABLE IF NOT EXISTS `input_variable_history` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL,
  `SendParamID` varchar(50) NOT NULL,
  `UploadParamID` varchar(50) NOT NULL,
  `ParamName` varchar(50) NOT NULL,
  `Type` varchar(50) NOT NULL,
  `ParamValueRatio` varchar(50) NOT NULL,
  `Model` varchar(50) NOT NULL,
  `HistoryMaxValue` varchar(50) DEFAULT NULL COMMENT '参考model设定值',
  `HistoryMinValue` varchar(50) DEFAULT NULL COMMENT '参考model设定值',
  `HistoryStandardValue` varchar(50) DEFAULT NULL COMMENT '参考model设定值',
  `ChangeMonitorValue` varchar(50) DEFAULT NULL,
  `ActualValue` varchar(50) DEFAULT NULL,
  `BycellOutputValue` varchar(50) DEFAULT NULL,
  `DataTime` datetime NOT NULL DEFAULT current_timestamp(),
  `LogFrom` varchar(50) NOT NULL DEFAULT 'download' COMMENT '当前记录是下载记录还是上载记录',
  `DownloadRemark` varchar(50) NOT NULL DEFAULT 'download' COMMENT 'input下发名称',
  PRIMARY KEY (`ID`),
  KEY `model` (`Model`),
  KEY `FK_input_variable_history_input_param_source` (`LogFrom`),
  CONSTRAINT `FK_input_variable_history_input_param_source` FOREIGN KEY (`LogFrom`) REFERENCES `input_log_from` (`source`)
) ENGINE=InnoDB AUTO_INCREMENT=637 DEFAULT CHARSET=utf8 COMMENT='input参数下发历史数据以及手动上传PLC的历史数据\r\nEQID为设备名称：Baking设备必填字段，因为每个炉子可能做不一样的Model,每个炉子下发的时间点不一样。\r\nDataTime表示从PLC上传的时间或者下发到PLC的时间\r\nmodel字段表示当前生产的型号\r\ninput_variableDID表示input_variable表里的主键ID\r\n则需要在user_fefine_variable表里建立20个控制温度变量，个对应1个炉子，每个控制温度变量绑定一个PLC。';

-- Dumping data for table yh_slix.input_variable_history: ~0 rows (大约)
/*!40000 ALTER TABLE `input_variable_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `input_variable_history` ENABLE KEYS */;

-- Dumping structure for table yh_slix.log4net
CREATE TABLE IF NOT EXISTS `log4net` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `logdate` datetime(3) NOT NULL COMMENT '日期时间',
  `loglevel` varchar(50) NOT NULL COMMENT 'log等级',
  `logger` varchar(50) DEFAULT NULL COMMENT 'log源',
  `message` mediumtext NOT NULL COMMENT '信息',
  `exception` mediumtext DEFAULT NULL COMMENT '报错',
  `softwareName` varchar(50) DEFAULT NULL COMMENT '软件名称',
  PRIMARY KEY (`id`),
  KEY `logdate` (`logdate`),
  KEY `loglevel` (`loglevel`),
  CONSTRAINT `FK_log4net_loglevel` FOREIGN KEY (`loglevel`) REFERENCES `loglevel` (`loglevel`)
) ENGINE=InnoDB AUTO_INCREMENT=4320 DEFAULT CHARSET=utf8 COMMENT='软件运行日志\r\nloglevel=INFO日志是会在在UI界面上显示查看的\r\nloglevel=DEBUG日志只是在txt格式的log文件里查看到\r\nloglevel=ERROR表示代码逻辑异常的日志\r\nloglevel=WARN表示警告提醒的日志信息\r\nloglevel=FATAl表示软件try catch到的异常信息';

-- Dumping data for table yh_slix.log4net: ~6 rows (大约)
/*!40000 ALTER TABLE `log4net` DISABLE KEYS */;
INSERT INTO `log4net` (`id`, `logdate`, `loglevel`, `logger`, `message`, `exception`, `softwareName`) VALUES
	(4314, '2020-09-18 16:30:40.230', 'INFO', '', '软件启动', '', 'PTF'),
	(4315, '2020-09-18 16:30:42.505', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(4316, '2020-09-18 16:30:47.646', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(4317, '2020-09-18 16:30:52.380', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(4318, '2020-09-18 16:30:52.380', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(4319, '2020-09-18 16:31:02.397', 'INFO', '', '连接备用服务器1号失败', '', 'PTF');
/*!40000 ALTER TABLE `log4net` ENABLE KEYS */;

-- Dumping structure for table yh_slix.loglevel
CREATE TABLE IF NOT EXISTS `loglevel` (
  `loglevel` varchar(50) NOT NULL,
  PRIMARY KEY (`loglevel`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='日志等级表';

-- Dumping data for table yh_slix.loglevel: ~5 rows (大约)
/*!40000 ALTER TABLE `loglevel` DISABLE KEYS */;
INSERT INTO `loglevel` (`loglevel`) VALUES
	('DEBUG'),
	('ERROR'),
	('FATAL'),
	('INFO'),
	('WARN');
/*!40000 ALTER TABLE `loglevel` ENABLE KEYS */;

-- Dumping structure for table yh_slix.log_operation
CREATE TABLE IF NOT EXISTS `log_operation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `datatime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `username` varchar(50) NOT NULL COMMENT '当前登录用户',
  `Action` varchar(200) NOT NULL COMMENT '操作内容 /MES ID变更内容',
  PRIMARY KEY (`id`),
  KEY `datatime` (`datatime`)
) ENGINE=InnoDB AUTO_INCREMENT=85 DEFAULT CHARSET=utf8 COMMENT='操作记录信息/参数变更记录信息\r\n比如操作了某个运行按钮，操作了停止按钮，需要记录在此表\r\n比如软件记录MES ID的值发生了变化，则也需要将MES ID变化前的值和变化后的值记录在此表';

-- Dumping data for table yh_slix.log_operation: ~1 rows (大约)
/*!40000 ALTER TABLE `log_operation` DISABLE KEYS */;
INSERT INTO `log_operation` (`id`, `datatime`, `username`, `Action`) VALUES
	(84, '2020-09-18 16:30:47.688', 'SuperAdmin', '本地登陆');
/*!40000 ALTER TABLE `log_operation` ENABLE KEYS */;

-- Dumping structure for table yh_slix.log_plc_interactive
CREATE TABLE IF NOT EXISTS `log_plc_interactive` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `datatime` datetime(3) NOT NULL COMMENT '记录时间',
  `variable_ID` int(11) NOT NULL COMMENT '变量ID',
  `value` varchar(200) NOT NULL DEFAULT '' COMMENT '读写值',
  `remark` varchar(100) NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `datatime` (`datatime`),
  KEY `FK_log_plc_interactive_user_fefine_variable` (`variable_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=1840 DEFAULT CHARSET=utf8 COMMENT='变量自动监控记录表';

-- Dumping data for table yh_slix.log_plc_interactive: ~0 rows (大约)
/*!40000 ALTER TABLE `log_plc_interactive` DISABLE KEYS */;
/*!40000 ALTER TABLE `log_plc_interactive` ENABLE KEYS */;

-- Dumping structure for table yh_slix.log_simple_mes_interface_execution
CREATE TABLE IF NOT EXISTS `log_simple_mes_interface_execution` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `datatime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `FunctionID` varchar(50) NOT NULL,
  `GUID` varchar(50) NOT NULL,
  `ResponseTime` datetime(3) DEFAULT NULL COMMENT '上位机上传数据给MES后，MES返回信息的时间，单位为ms',
  `RequestTime` datetime(3) DEFAULT NULL,
  `Data` mediumtext NOT NULL,
  `errorMsg` mediumtext DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `GUID` (`GUID`),
  KEY `FunctionID` (`FunctionID`)
) ENGINE=InnoDB AUTO_INCREMENT=305 DEFAULT CHARSET=utf8 COMMENT='MES接口上传数据表，包括了上传MES的信息和MES返回的信息\r\nResoponseTime:上位机上传数据给MES后，MES返回信息的时间，单位为ms';

-- Dumping data for table yh_slix.log_simple_mes_interface_execution: ~0 rows (大约)
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` DISABLE KEYS */;
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` ENABLE KEYS */;

-- Dumping structure for table yh_slix.ngcode
CREATE TABLE IF NOT EXISTS `ngcode` (
  `NGCode` varchar(50) NOT NULL COMMENT 'NG代码',
  `NGreason` varchar(50) NOT NULL COMMENT 'NG原因',
  PRIMARY KEY (`NGCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='NGcode原因表';

-- Dumping data for table yh_slix.ngcode: ~2 rows (大约)
/*!40000 ALTER TABLE `ngcode` DISABLE KEYS */;
INSERT INTO `ngcode` (`NGCode`, `NGreason`) VALUES
	('E-001', '电池超温'),
	('OK', 'OK');
/*!40000 ALTER TABLE `ngcode` ENABLE KEYS */;

-- Dumping structure for table yh_slix.offline_mes_interface_upload_buffer_datas
CREATE TABLE IF NOT EXISTS `offline_mes_interface_upload_buffer_datas` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(20) NOT NULL,
  `logDateTime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `FunctionID` varchar(50) NOT NULL,
  `Guid` varchar(50) NOT NULL,
  `Data` mediumtext NOT NULL,
  `cellbarcode` mediumtext NOT NULL,
  PRIMARY KEY (`id`),
  KEY `logDateTime` (`logDateTime`),
  KEY `FunctionID` (`FunctionID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='离线数据缓存，待上传';

-- Dumping data for table yh_slix.offline_mes_interface_upload_buffer_datas: ~0 rows (大约)
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` DISABLE KEYS */;
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` ENABLE KEYS */;

-- Dumping structure for table yh_slix.permissions
CREATE TABLE IF NOT EXISTS `permissions` (
  `PermissionId` int(11) NOT NULL COMMENT '主键',
  `PermissionName` varchar(200) NOT NULL COMMENT '菜单名称',
  `PermissionCode` varchar(200) DEFAULT NULL COMMENT '菜单编码',
  `Uri` varchar(200) DEFAULT NULL,
  `Remark` varchar(50) DEFAULT NULL COMMENT '备注',
  `DisplayOrder` int(11) NOT NULL COMMENT '显示顺序',
  `ParentId` int(11) DEFAULT NULL COMMENT '父菜单ID',
  `Depth` int(11) NOT NULL COMMENT '层级',
  PRIMARY KEY (`PermissionId`),
  UNIQUE KEY `PermissionCode` (`PermissionCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table yh_slix.permissions: ~46 rows (大约)
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` (`PermissionId`, `PermissionName`, `PermissionCode`, `Uri`, `Remark`, `DisplayOrder`, `ParentId`, `Depth`) VALUES
	(0, '设备概况', 'DeviceOverview', '', 'qq', 1, NULL, 1),
	(1, '设备概况', 'DeviceOverview.ProductOverview', 'pack://application:,,,/ATL_MES;component/ATL/ProductOverviewPage.xaml', '', 1, 0, 2),
	(2, '生产监控', 'DeviceOverview.Monitor', NULL, '', 2, 0, 2),
	(3, '生产数据历史统计', 'DeviceOverview.DataCapacityStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/DataCapacityStatisticsPage.xaml', '', 3, 0, 2),
	(4, 'NG电芯历史统计', 'DeviceOverview.NGStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/NGStatisticsPage.xaml', '', 4, 0, 2),
	(5, '报警统计', 'DeviceOverview.AlarmStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/AlarmStatisticsPage.xaml', '', 5, 0, 2),
	(6, 'PC/PLC交互数据', 'DeviceOverview.PC-PLCrealtimeData', 'pack://application:,,,/ATL.UI;component/DeviceOverview/PC-PLCrealtimeDataPage.xaml', NULL, 6, 0, 2),
	(7, '软件版本信息', 'DeviceOverview.Version', 'pack://application:,,,/ATL.UI;component/DeviceOverview/VersionPage.xaml', '', 7, 0, 2),
	(8, '异常处理', 'DeviceOverview.Start', 'pack://application:,,,/ATL.UI;component/DeviceOverview/StartPage.xaml', '', 8, 0, 2),
	(20, 'MES系统', 'MES', NULL, '', 2, NULL, 1),
	(21, 'MES网页', 'MES.MESweb', 'pack://application:,,,/ATL.UI;component/MES/MesWebPage.xaml', '', 1, 20, 2),
	(22, 'Input参数对比下发', 'MES.InputParaDownload', 'pack://application:,,,/ATL.UI;component/MES/InputParaDownloadPage.xaml', '', 2, 20, 2),
	(23, 'MES接口测试', 'MES.InerfaceTest', 'pack://application:,,,/ATL.UI;component/MES/InerfaceTestPage.xaml', '', 3, 20, 2),
	(40, '设备维护', 'Maintain', NULL, '', 3, NULL, 1),
	(41, '实时报警', 'Maintain.RealTime', 'pack://application:,,,/ATL.UI;component/Maintain/RealTimeAlarmPage.xaml', '', 1, 40, 2),
	(42, '历史报警', 'Maintain.History', 'pack://application:,,,/ATL.UI;component/Maintain/HistoryAlarmPage.xaml', '', 2, 40, 2),
	(43, '设备自检', 'Maintain.SpotInspection', 'pack://application:,,,/ATL.UI;component/Maintain/SpotInspectionPage.xaml', NULL, 3, 40, 2),
	(44, '易损件维护', 'Maintain.VulnerableStatistic', 'pack://application:,,,/ATL.UI;component/Maintain/VulnerableStatisticPage.xaml', NULL, 4, 40, 2),
	(80, '系统配置', 'SystemSetting', NULL, '', 5, NULL, 1),
	(81, '菜单管理', 'SystemSetting.Menus', 'pack://application:,,,/ATL.UI;component/SystemSetting/MenuManagementPage.xaml', '软件工程师才可看', 1, 80, 2),
	(82, '参数配置', 'SystemSetting.ParameterConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/ParameterConfigPage.xaml', NULL, 2, 80, 2),
	(83, 'PLC读取设置', 'SystemSetting.PLCcommConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/PLCconfigPage.xaml', '系统管理员可视', 3, 80, 2),
	(84, '变量定义', 'SystemSetting.UserSettingParameter', 'pack://application:,,,/ATL.UI;component/SystemSetting/UserSettingParameterPage.xaml', NULL, 4, 80, 2),
	(86, '报警信息维护', 'SystemSetting.AlarmConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/AlarmConfigPage.xaml', '', 6, 80, 2),
	(87, 'PC/PLC交互数据配置', 'SystemSetting.PLCPCinteractive', 'pack://application:,,,/ATL.UI;component/SystemSetting/PLCPCinteractivePage.xaml', NULL, 7, 80, 2),
	(88, '设备自检参数配置', 'SystemSetting.SpotInspectionConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/SpotInspectionConfigPage.xaml', NULL, 8, 80, 2),
	(89, 'Input/Output参数配置', 'SystemSetting.InputConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/InputConfigPage.xaml', '', 9, 80, 2),
	(90, '易损件地址配置', 'SystemSetting.VulnerableAddressConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/VulnerableAddressConfigPage.xaml', NULL, 10, 80, 2),
	(91, '系统配置', 'SystemSetting.SystemConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/SystemConfig.xaml', NULL, 11, 80, 2),
	(100, '日志查询', 'LogQuery', NULL, '', 6, NULL, 1),
	(101, 'MES接口调用记录', 'LogQuery.MESinterface', 'pack://application:,,,/ATL.UI;component/LogQuery/MESinterfacePage.xaml', NULL, 1, 100, 2),
	(102, 'PC/PLC交互记录', 'LogQuery.PLCinteractive', 'pack://application:,,,/ATL.UI;component/LogQuery/PLCinteractivePage.xaml', NULL, 2, 100, 2),
	(103, '重要参数变化记录', 'LogQuery.HMIparachanged', 'pack://application:,,,/ATL.UI;component/LogQuery/HMIparachanged.xaml', '', 3, 100, 2),
	(104, 'Input下发记录', 'LogQuery.HistoryInputOutput', 'pack://application:,,,/ATL.UI;component/LogQuery/HistoryInputOutput.xaml', '', 4, 100, 2),
	(105, '操作日志', 'LogQuery.Operation', 'pack://application:,,,/ATL.UI;component/LogQuery/OperationPage.xaml', NULL, 5, 100, 2),
	(106, '电池生产记录', 'LogQuery.ProductionData', 'pack://application:,,,/ATL.UI;component/LogQuery/ProductionData.xaml', NULL, 6, 100, 2),
	(107, '运行日志查询', 'LogQuery.SoftwareLogReport', 'pack://application:,,,/ATL.UI;component/LogQuery/SoftwareLogReport.xaml', NULL, 7, 100, 2),
	(120, '用户管理', 'UserManager', NULL, '', 7, NULL, 1),
	(121, '用户登录', 'UserManager.Login', 'pack://application:,,,/ATL.UI;component/UserManager/LoginManagerPage.xaml', '', 1, 120, 2),
	(122, '个人管理', 'UserManager.Personal', 'pack://application:,,,/ATL.UI;component/UserManager/PersonalInfoPage.xaml', NULL, 2, 120, 2),
	(123, '用户管理', 'UserManager.User', 'pack://application:,,,/ATL.UI;component/UserManager/UserManagerPage.xaml', NULL, 3, 120, 2),
	(124, '角色管理', 'UserManager.Role', 'pack://application:,,,/ATL.UI;component/UserManager/RoleManagerPage.xaml', NULL, 4, 120, 2),
	(140, '设备控制', 'DeviceControl', NULL, '', 8, NULL, 1),
	(141, '整线概况', 'DeviceControl.DeviceDebug', 'pack://application:,,,/ATL_MES;component/ATL/DeviceDebug.xaml', '', 1, 140, 2),
	(142, '注液机上位机', 'DeviceControl.WinformMainPage', 'pack://application:,,,/PTF;component/HBinjectPage.xaml', NULL, 3, 140, 2),
	(143, 'CCD', 'DeviceControl.WinformUserControlPage', 'pack://application:,,,/PTF;component/HBvisionPage.xaml', NULL, 4, 140, 2);
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_area_config
CREATE TABLE IF NOT EXISTS `plc_area_config` (
  `areaName` varchar(50) NOT NULL COMMENT '区域名称',
  `bitLength` int(11) DEFAULT NULL COMMENT '区域位长度',
  `brand` varchar(50) DEFAULT NULL COMMENT '品牌',
  PRIMARY KEY (`areaName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC软元件区域配置';

-- Dumping data for table yh_slix.plc_area_config: ~9 rows (大约)
/*!40000 ALTER TABLE `plc_area_config` DISABLE KEYS */;
INSERT INTO `plc_area_config` (`areaName`, `bitLength`, `brand`) VALUES
	('A', 16, 'omron'),
	('B', 8, 'Beckoff'),
	('C', 16, 'omron'),
	('D', 16, 'melsec,keyence,Panasonic,omron'),
	('DB', 8, 'siemens'),
	('H', 16, 'omron'),
	('M', 1, 'melsec,siemens,keyence,Panasonic'),
	('R', 16, 'keyence,Panasonic'),
	('W', 16, 'omron');
/*!40000 ALTER TABLE `plc_area_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_brand
CREATE TABLE IF NOT EXISTS `plc_brand` (
  `brand` varchar(50) NOT NULL,
  PRIMARY KEY (`brand`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC品牌';

-- Dumping data for table yh_slix.plc_brand: ~7 rows (大约)
/*!40000 ALTER TABLE `plc_brand` DISABLE KEYS */;
INSERT INTO `plc_brand` (`brand`) VALUES
	('AB'),
	('Beckhoff'),
	('Keyence'),
	('Melsec'),
	('Omron'),
	('Panasonic'),
	('Siemens');
/*!40000 ALTER TABLE `plc_brand` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_config
CREATE TABLE IF NOT EXISTS `plc_config` (
  `plc_ID` int(11) NOT NULL AUTO_INCREMENT,
  `plcName` varchar(30) NOT NULL COMMENT 'PLC名称',
  `deviceName` varchar(30) DEFAULT NULL COMMENT '设备名称',
  `address` varchar(20) DEFAULT NULL COMMENT 'IP地址或者串口号',
  `address_para` varchar(50) DEFAULT NULL COMMENT 'IP端口号或串口通信参数',
  `ProtocolName` varchar(50) DEFAULT NULL,
  `remark` varchar(50) DEFAULT NULL,
  `enabled` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`plc_ID`),
  UNIQUE KEY `deviceName` (`plcName`),
  KEY `FK_plc_config_plc_protocol` (`ProtocolName`),
  CONSTRAINT `FK_plc_config_plc_protocol` FOREIGN KEY (`ProtocolName`) REFERENCES `plc_protocol` (`ProtocolName`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='PLC通信配置表';

-- Dumping data for table yh_slix.plc_config: ~1 rows (大约)
/*!40000 ALTER TABLE `plc_config` DISABLE KEYS */;
INSERT INTO `plc_config` (`plc_ID`, `plcName`, `deviceName`, `address`, `address_para`, `ProtocolName`, `remark`, `enabled`) VALUES
	(1, 'PLC1', 'SLIX', '10.217.99.204', '9600', 'Fins_Tcp', NULL, 1);
/*!40000 ALTER TABLE `plc_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_interactive_invariables
CREATE TABLE IF NOT EXISTS `plc_interactive_invariables` (
  `did` int(11) NOT NULL AUTO_INCREMENT,
  `variableID` int(11) NOT NULL,
  `orderID` int(11) NOT NULL COMMENT '排序显示',
  `autoSave` int(1) unsigned NOT NULL DEFAULT 0 COMMENT '1表示该变量变化后自动保存记录到log_plc_interactive表里',
  PRIMARY KEY (`did`),
  KEY `FK_plc_interactive_invariables_user_fefine_variable` (`variableID`),
  KEY `orderID` (`orderID`),
  CONSTRAINT `FK_plc_interactive_invariables_user_define_variable` FOREIGN KEY (`variableID`) REFERENCES `user_define_variable` (`variableID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='上位机-PLC交互变量集合';

-- Dumping data for table yh_slix.plc_interactive_invariables: ~0 rows (大约)
/*!40000 ALTER TABLE `plc_interactive_invariables` DISABLE KEYS */;
/*!40000 ALTER TABLE `plc_interactive_invariables` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_protocol
CREATE TABLE IF NOT EXISTS `plc_protocol` (
  `ProtocolName` varchar(50) NOT NULL COMMENT '通信协议',
  `model` varchar(100) DEFAULT NULL COMMENT '系列',
  `remark` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ProtocolName`),
  KEY `ProtocolName` (`ProtocolName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC通信协议';

-- Dumping data for table yh_slix.plc_protocol: ~22 rows (大约)
/*!40000 ALTER TABLE `plc_protocol` DISABLE KEYS */;
INSERT INTO `plc_protocol` (`ProtocolName`, `model`, `remark`) VALUES
	('ADS', 'Beckhoff', '每种通信协议有固定的地址读写区域'),
	('ADS_D', 'Beckhoff', '采用偏移量读写方法来批量操作'),
	('A_1ENet_Ascii', 'MelsecMcNet', NULL),
	('A_1ENet_Binary', 'MelsecMcNet', NULL),
	('CIP', 'Omron NX-701,AB', NULL),
	('Fins_HostLink', 'Omron', '每种通信协议有固定的地址读写区域'),
	('Fins_Tcp', 'Omron', NULL),
	('MC_3E_Ascii', 'MelsecMcAsciiNet,Keyence', NULL),
	('MC_3E_Binary', 'MelsecMcNet,Keyence,Panasonic', NULL),
	('MelsecA1ENet', 'Melsec', NULL),
	('Mewtocol', 'Panasonic', NULL),
	('MewtocolOverTcp', 'Panasonic', NULL),
	('ModbusAscii', NULL, NULL),
	('ModbusRtu', NULL, NULL),
	('ModbusTcp', NULL, NULL),
	('Nano_Ascii', 'Keyence', NULL),
	('S7Net_1200', 'S7-1200', ''),
	('S7Net_1500', 'S7-1500', ''),
	('S7Net_200', 'S7-200', 'S7-200没有DB区'),
	('S7Net_200Smart', 'S7-200Smart', ''),
	('S7Net_300', 'S7-300', ''),
	('S7Net_400', 'S7-400', '');
/*!40000 ALTER TABLE `plc_protocol` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_rw_config
CREATE TABLE IF NOT EXISTS `plc_rw_config` (
  `plc_rw_area_did` int(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL COMMENT '地址段名称',
  `plcID` int(11) NOT NULL COMMENT '必须为PLC编号',
  `areaName` varchar(50) DEFAULT NULL COMMENT '读取的区域',
  `startAddress` varchar(50) DEFAULT NULL COMMENT '起始地址',
  `length` int(11) NOT NULL COMMENT '数据长度',
  `rw` varchar(20) NOT NULL,
  `cycle` int(11) DEFAULT NULL COMMENT '读写周期(ms)',
  `lastTime` datetime NOT NULL DEFAULT current_timestamp() COMMENT '上一次读写时间点',
  `enabled` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`plc_rw_area_did`),
  KEY `FK_plc_rw_config_plc_area_config` (`areaName`),
  KEY `FK_plc_rw_config_plc_rw` (`rw`),
  KEY `FK_plc_rw_config_device_config` (`plcID`),
  CONSTRAINT `FK_plc_rw_config_device_config` FOREIGN KEY (`plcID`) REFERENCES `plc_config` (`plc_ID`),
  CONSTRAINT `FK_plc_rw_config_plc_area_config` FOREIGN KEY (`areaName`) REFERENCES `plc_area_config` (`areaName`),
  CONSTRAINT `FK_plc_rw_config_plc_rw_field` FOREIGN KEY (`rw`) REFERENCES `plc_rw_field` (`rw`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8 COMMENT='PLC读写区域设置';

-- Dumping data for table yh_slix.plc_rw_config: ~16 rows (大约)
/*!40000 ALTER TABLE `plc_rw_config` DISABLE KEYS */;
INSERT INTO `plc_rw_config` (`plc_rw_area_did`, `name`, `plcID`, `areaName`, `startAddress`, `length`, `rw`, `cycle`, `lastTime`, `enabled`) VALUES
	(1, '设备output数据', 1, 'D', '16000', 46, 'R', 2000, '2019-12-18 14:29:17', 1),
	(2, '相关验证数据', 1, 'D', '16070', 20, 'R', 1000, '2019-12-18 14:29:17', 1),
	(3, '设备状态数据', 1, 'D', '16090', 20, 'R', 1000, '2019-12-18 14:29:17', 1),
	(4, '设备报警数据', 1, 'D', '16100', 20, 'R', 1000, '2019-12-18 14:29:17', 1),
	(5, '设备关键件寿命实际值', 1, 'D', '16150', 40, 'R', 1000, '2019-12-18 14:29:17', 1),
	(6, '设备履历数据', 1, 'D', '16200', 50, 'R', 1000, '2019-12-18 14:29:17', 1),
	(7, 'MES主动采集数据', 1, 'D', '16260', 20, 'R', 1000, '2019-12-18 14:29:17', 1),
	(8, '发送测量设备点检数据', 1, 'D', '16300', 30, 'R', 1000, '2019-12-18 14:29:17', 1),
	(9, 'input参数监控值', 1, 'D', '16330', 100, 'R', 1000, '2019-12-18 14:29:17', 1),
	(10, 'input参数实际值', 1, 'D', '16500', 40, 'R', 5000, '2019-12-18 14:29:17', 1),
	(11, 'input参数下发值', 1, 'D', '18180', 200, 'R', 1000, '2019-12-18 14:29:17', 1),
	(12, '定时信号', 1, 'D', '18000', 20, 'W', 1000, '2019-12-18 14:29:17', 1),
	(13, '设备控制指令', 1, 'D', '18050', 20, 'W', 1000, '2019-12-18 14:29:17', 1),
	(14, '设备关键件设定值', 1, 'D', '18100', 40, 'R', 1000, '2019-12-18 14:29:17', 1),
	(22, '软件版本号', 1, 'D', '18240', 10, 'R', 1000, '2019-12-18 14:29:17', 1),
	(23, 'input参数是否下发对比', 1, 'D', '17000', 10, 'R', 1000, '2019-12-18 14:29:17', 1);
/*!40000 ALTER TABLE `plc_rw_config` ENABLE KEYS */;

-- Dumping structure for table yh_slix.plc_rw_field
CREATE TABLE IF NOT EXISTS `plc_rw_field` (
  `rw` varchar(20) NOT NULL COMMENT '设置读或写',
  `remark` varchar(20) NOT NULL COMMENT '备注',
  PRIMARY KEY (`rw`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC操作选择';

-- Dumping data for table yh_slix.plc_rw_field: ~3 rows (大约)
/*!40000 ALTER TABLE `plc_rw_field` DISABLE KEYS */;
INSERT INTO `plc_rw_field` (`rw`, `remark`) VALUES
	('NotRW', '非周期性读写区域'),
	('R', '读持续取区域'),
	('W', '持续写入区域');
/*!40000 ALTER TABLE `plc_rw_field` ENABLE KEYS */;

-- Dumping structure for table yh_slix.production_data
CREATE TABLE IF NOT EXISTS `production_data` (
  `Iden` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `EquipmentID` varchar(50) NOT NULL,
  `ProductSN` varchar(50) NOT NULL COMMENT '电池条码',
  `Model` varchar(50) NOT NULL COMMENT '生产型号',
  `NgCode` varchar(50) DEFAULT '' COMMENT 'NG代码',
  `NGreason` varchar(255) DEFAULT NULL COMMENT '不良原因',
  `ProductDate` datetime NOT NULL DEFAULT current_timestamp() COMMENT '生产时间',
  `InTime` datetime NOT NULL DEFAULT current_timestamp(),
  `OutTime` datetime NOT NULL DEFAULT current_timestamp(),
  `Operator` varchar(50) NOT NULL COMMENT '员工号',
  `ResourceShift` varchar(50) NOT NULL COMMENT '班次',
  `CellOrderID` int(11) DEFAULT NULL COMMENT '电芯序号',
  `CarrierBarcode` varchar(50) DEFAULT NULL COMMENT '弹夹条码',
  `BindResult` varchar(50) DEFAULT NULL COMMENT 'MES返回的绑定结果',
  `remarks` int(11) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`Iden`),
  KEY `index_marks` (`NgCode`),
  KEY `index_batteryBarCode` (`ProductSN`),
  KEY `Model` (`Model`),
  KEY `ProductDate` (`ProductDate`),
  CONSTRAINT `FK_production_data_ngcode` FOREIGN KEY (`NgCode`) REFERENCES `ngcode` (`NGCode`)
) ENGINE=InnoDB AUTO_INCREMENT=39485 DEFAULT CHARSET=utf8 COMMENT='每个工艺的设备的电池数据会不一致，需要各个厂家自己设计此表\r\n必须要有EquipmentID、条码字段ProductSN、品质结果字段Model、NgCode、生产时间字段ProductDate、InTime、OutTime、Operator、ResourceShift字段\r\n生产时间字段ProductDate的内容必填\r\n对于有上料扫码的设备，则InTime字段内容必填，表示上料扫码的时间\r\n对于能够知道电池下料时候的时间的设备，则OutTime字段内容必填，表示电池离开时候的时间。\r\nOperator表示当前登录MES账户\r\nResourceShift表示MES下发的当前班次号';

-- Dumping data for table yh_slix.production_data: ~0 rows (大约)
/*!40000 ALTER TABLE `production_data` DISABLE KEYS */;
/*!40000 ALTER TABLE `production_data` ENABLE KEYS */;

-- Dumping structure for table yh_slix.production_ngcode_statistics
CREATE TABLE IF NOT EXISTS `production_ngcode_statistics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `model` varchar(50) NOT NULL,
  `MCCollectDDate` datetime NOT NULL COMMENT '统计班次时间',
  `ME` varchar(50) NOT NULL COMMENT 'M/E 白班或晚班',
  `NGCode` varchar(50) NOT NULL COMMENT 'NG Code',
  `Count` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `model` (`model`),
  KEY `NGCode` (`NGCode`),
  CONSTRAINT `FK_production_ngcode_statistics_ngcode` FOREIGN KEY (`NGCode`) REFERENCES `ngcode` (`NGCode`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='每个班次不同NG原因数量统计';

-- Dumping data for table yh_slix.production_ngcode_statistics: ~1 rows (大约)
/*!40000 ALTER TABLE `production_ngcode_statistics` DISABLE KEYS */;
INSERT INTO `production_ngcode_statistics` (`id`, `model`, `MCCollectDDate`, `ME`, `NGCode`, `Count`) VALUES
	(1, 'M20', '2019-07-20 10:04:33', 'M', 'E-001', 3566);
/*!40000 ALTER TABLE `production_ngcode_statistics` ENABLE KEYS */;

-- Dumping structure for table yh_slix.production_statistics
CREATE TABLE IF NOT EXISTS `production_statistics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `model` varchar(50) NOT NULL,
  `MCCollectDDate` datetime NOT NULL COMMENT '统计班次时间',
  `ME` varchar(50) NOT NULL COMMENT 'M/E 白班或晚班',
  `NGcount` int(11) NOT NULL COMMENT 'NG数量',
  `OKcount` int(11) NOT NULL COMMENT 'OK数量',
  `InCount` int(11) NOT NULL COMMENT '来料总数量',
  `OKRate` float(10,2) NOT NULL COMMENT '优率：合格百分比',
  `datatime` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `model` (`model`),
  KEY `MCCollectDDate` (`MCCollectDDate`)
) ENGINE=InnoDB AUTO_INCREMENT=905 DEFAULT CHARSET=utf8 COMMENT='班次产量统计';

-- Dumping data for table yh_slix.production_statistics: ~30 rows (大约)
/*!40000 ALTER TABLE `production_statistics` DISABLE KEYS */;
INSERT INTO `production_statistics` (`id`, `model`, `MCCollectDDate`, `ME`, `NGcount`, `OKcount`, `InCount`, `OKRate`, `datatime`) VALUES
	(87, 'M20', '2019-07-15 08:00:00', 'M', 0, 0, 0, 0.00, '2017-09-15 18:40:04'),
	(257, 'M20', '2019-07-16 08:00:00', 'M', 9, 0, 963, 0.00, '2017-09-16 20:01:37'),
	(730, 'M20', '2019-07-16 08:00:00', 'M', 9, 0, 963, 0.00, '2017-09-16 20:01:37'),
	(731, 'M20', '2019-07-15 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-16 07:59:59'),
	(733, 'M21', '2019-07-16 20:00:00', 'E', 0, 0, 120, 0.00, '2017-09-17 08:02:30'),
	(851, 'M23', '2019-07-17 08:00:00', 'M', 0, 0, 0, 0.00, '2017-09-17 20:01:53'),
	(852, 'M21', '2019-07-17 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-18 08:01:46'),
	(853, 'M21', '2019-07-18 08:00:00', 'M', 1, 0, 658, 0.00, '2017-09-18 20:01:38'),
	(863, 'M21', '2017-09-18 20:00:00', 'E', 1, 0, 658, 0.00, '2017-09-19 08:00:01'),
	(867, 'M20', '2017-09-19 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-20 07:59:59'),
	(868, 'M22', '2017-09-20 08:00:00', 'M', 12, 0, 575, 0.00, '2017-09-20 20:00:04'),
	(869, 'M22', '2017-09-20 20:00:00', 'E', 12, 0, 575, 0.00, '2017-09-20 20:23:34'),
	(870, 'M22', '2017-09-21 08:00:00', 'M', 164, 0, 1843, 0.00, '2017-09-21 19:59:41'),
	(874, 'M22', '2017-09-21 20:00:00', 'E', 164, 0, 1843, 0.00, '2017-09-22 08:00:04'),
	(875, 'M20', '2017-09-22 08:00:00', 'M', 0, 0, 0, 0.00, '2017-09-22 19:28:12'),
	(881, 'M20', '2017-09-24 08:00:00', 'M', 1, 0, 519, 0.00, '2017-09-24 20:00:11'),
	(882, 'M20', '2017-09-23 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-24 07:59:59'),
	(883, 'M20', '2017-09-24 20:00:00', 'E', 1, 0, 519, 0.00, '2017-09-25 08:00:04'),
	(884, 'M20', '2017-09-25 08:00:00', 'M', 1, 0, 717, 0.00, '2017-09-25 19:59:54'),
	(888, 'M20', '2017-09-25 20:00:00', 'E', 1, 0, 717, 0.00, '2017-09-26 08:00:17'),
	(889, 'M20', '2017-09-26 08:00:00', 'M', 1, 0, 717, 0.00, '2017-09-26 15:24:31'),
	(891, 'M20', '2017-09-26 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-26 20:23:27'),
	(892, 'M20', '2017-09-27 08:00:00', 'M', 0, 0, 0, 0.00, '2017-09-27 08:37:16'),
	(896, 'M20', '2017-09-28 08:00:00', 'M', 0, 0, 0, 0.00, '2017-09-28 09:10:07'),
	(897, 'M20', '2017-09-27 20:00:00', 'E', 0, 0, 0, 0.00, '2017-09-28 07:59:59'),
	(898, 'M20', '2019-07-06 08:00:00', 'M', 0, 0, 0, 0.00, '2017-10-06 09:23:38'),
	(899, 'M20', '2019-07-05 20:00:00', 'E', 0, 0, 0, 0.00, '2017-10-06 07:59:59'),
	(902, 'M20', '2019-07-06 20:00:00', 'E', 0, 0, 0, 0.00, '2017-10-06 21:38:58'),
	(903, 'M20', '2019-07-07 08:00:00', 'M', 0, 0, 0, 0.00, '2017-10-07 08:00:16'),
	(904, 'M21', '2019-07-06 20:00:00', 'E', 0, 0, 0, 0.00, '2017-10-06 21:38:58');
/*!40000 ALTER TABLE `production_statistics` ENABLE KEYS */;

-- Dumping structure for table yh_slix.quickwearparthistoryinfo
CREATE TABLE IF NOT EXISTS `quickwearparthistoryinfo` (
  `ID` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `UploadParamID` varchar(50) NOT NULL COMMENT '易损件ID',
  `ParamName` varchar(50) NOT NULL COMMENT '易损件名称',
  `Type` varchar(20) NOT NULL COMMENT 'FLOAT，INT32',
  `SpartExpectedLifetime` int(11) NOT NULL COMMENT 'MES下发的可使用时间',
  `UsedLife` varchar(20) NOT NULL COMMENT '零部件已使用时间',
  `PercentWarning` float(11,2) NOT NULL COMMENT '预警阈值',
  `ParamValueRatio` float(11,2) NOT NULL COMMENT '倍率',
  `StartDate` datetime NOT NULL COMMENT '更换时间-开始使用的时间',
  `EndDate` datetime NOT NULL COMMENT '被更换掉的时间',
  `StartUser` varchar(20) DEFAULT NULL COMMENT '更换人',
  `EndUser` varchar(20) DEFAULT NULL COMMENT '更换人',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COMMENT='易损件下线记录';

-- Dumping data for table yh_slix.quickwearparthistoryinfo: ~2 rows (大约)
/*!40000 ALTER TABLE `quickwearparthistoryinfo` DISABLE KEYS */;
INSERT INTO `quickwearparthistoryinfo` (`ID`, `EquipmentID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `UsedLife`, `PercentWarning`, `ParamValueRatio`, `StartDate`, `EndDate`, `StartUser`, `EndUser`) VALUES
	(1, 'Baking001', '00022', '气缸1', 'Float', 1000, '200.20', 0.95, 1.00, '2019-07-22 10:32:19', '2019-08-22 10:32:19', 'admin1', 'admin2'),
	(2, 'Baking002', '00022', '同步带', 'Float', 1000, '100.35', 0.90, 1.00, '2019-09-22 10:32:19', '2019-10-02 10:32:19', 'admin1', 'admin2');
/*!40000 ALTER TABLE `quickwearparthistoryinfo` ENABLE KEYS */;

-- Dumping structure for table yh_slix.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `roleId` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `roleName` varchar(50) NOT NULL COMMENT '角色名称',
  `MesUserLevel` varchar(50) NOT NULL COMMENT 'MES里的权限内容',
  `UserLevelPLCValue` int(11) NOT NULL DEFAULT 0,
  `permissionCodes` varchar(10000) DEFAULT NULL COMMENT '菜单权限编码',
  `createTime` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建时间',
  `modifyTime` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `remark` varchar(250) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`roleId`),
  UNIQUE KEY `roleName` (`roleName`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- Dumping data for table yh_slix.roles: ~4 rows (大约)
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` (`roleId`, `roleName`, `MesUserLevel`, `UserLevelPLCValue`, `permissionCodes`, `createTime`, `modifyTime`, `remark`) VALUES
	(0, 'Development Authority', 'Administrator', 408, 'ALL', '2019-07-02 11:04:39', '2020-09-18 16:30:41', '超级管理员,禁止删除'),
	(1, 'Maintain Authority', 'Maintainer', 307, 'ALL', '2019-07-02 11:04:39', '2020-09-18 16:30:41', '系统管理员'),
	(2, 'Operator Authority', 'Operator', 206, 'UserManager,UserManager.Personal,UserManager.User', '2019-07-09 09:50:00', '2020-09-18 16:30:41', 'ME人员'),
	(10, 'Guest Authority', 'Guest', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,MES,MES.MESweb,Maintain,DataWareHouse,SystemSetting,LogQuery,UserManager,UserManager.Login,DeviceControl', '2019-07-02 11:04:39', '2020-09-18 16:30:41', '尚未登陆');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;

-- Dumping structure for table yh_slix.server_backpara
CREATE TABLE IF NOT EXISTS `server_backpara` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `backupServerIpAdr` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用MES服务端IP地址',
  `backupServerUdpPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用MES服务端udp接收端口号',
  `backupLocalUdpSendPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机udp发送端口',
  `backupLocalUdpRecvPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机tcp接收端口号',
  `backupLocalTcpPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机udp接收端口号',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- Dumping data for table yh_slix.server_backpara: ~1 rows (大约)
/*!40000 ALTER TABLE `server_backpara` DISABLE KEYS */;
INSERT INTO `server_backpara` (`ID`, `backupServerIpAdr`, `backupServerUdpPortNo`, `backupLocalUdpSendPortNo`, `backupLocalUdpRecvPortNo`, `backupLocalTcpPortNo`) VALUES
	(1, '172.23.11.65', '50030', '61031', '62030', '60032');
/*!40000 ALTER TABLE `server_backpara` ENABLE KEYS */;

-- Dumping structure for table yh_slix.users
CREATE TABLE IF NOT EXISTS `users` (
  `userId` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `userName` varchar(50) NOT NULL COMMENT '帐号',
  `password` varchar(255) NOT NULL COMMENT '密码',
  `name` varchar(50) NOT NULL COMMENT '姓名',
  `roleId` int(11) NOT NULL COMMENT '角色编号',
  `createTime` datetime NOT NULL COMMENT '创建时间',
  `gender` int(11) DEFAULT NULL COMMENT '性别',
  `LastLoginTime` datetime DEFAULT NULL COMMENT '最后登录时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`userId`),
  KEY `fk_user_roles_roleId` (`roleId`),
  CONSTRAINT `FK_users_roles` FOREIGN KEY (`roleId`) REFERENCES `roles` (`roleId`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Dumping data for table yh_slix.users: ~3 rows (大约)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`userId`, `userName`, `password`, `name`, `roleId`, `createTime`, `gender`, `LastLoginTime`, `remark`) VALUES
	(1, 'admin', 'admin', 'admin', 1, '2017-03-23 21:05:21', 1, '2019-07-09 08:20:38', NULL),
	(2, 'Guest', 'Guest', 'Guest', 10, '2017-03-23 21:05:21', 1, '2020-09-18 16:30:41', NULL),
	(4, 'SuperAdmin', 'SuperAdmin', 'SuperAdmin', 0, '2019-07-08 13:22:06', 0, '2020-09-18 16:30:47', NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

-- Dumping structure for table yh_slix.user_define_variable
CREATE TABLE IF NOT EXISTS `user_define_variable` (
  `variableID` int(11) NOT NULL AUTO_INCREMENT,
  `variableName` varchar(50) NOT NULL COMMENT '变量名',
  `variableTypeID` int(11) NOT NULL COMMENT '变量类型,不同UI界面显示对应的值不一样',
  `value` varchar(50) DEFAULT NULL COMMENT '当前值',
  `description` varchar(50) NOT NULL COMMENT '名称',
  `valueTypes` varchar(50) NOT NULL COMMENT '变量类型',
  `variableLength` int(10) NOT NULL DEFAULT 1,
  `plc_rw_area_did` int(10) DEFAULT NULL COMMENT '地址段',
  `plc_address` varchar(50) DEFAULT NULL COMMENT 'PLC寄存器地址',
  `needMonitorLog` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否为HMI监控参数',
  `remark` varchar(100) DEFAULT NULL COMMENT '备注',
  `datatime` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`variableID`),
  UNIQUE KEY `variableName` (`variableName`),
  KEY `FK_user_fefine_variable_variable_types` (`valueTypes`),
  KEY `FK_user_fefine_variable_plc_rw_config` (`plc_rw_area_did`),
  KEY `FK_user_fefine_variable_variabletype` (`variableTypeID`),
  CONSTRAINT `FK_user_define_variable_valuetypes` FOREIGN KEY (`valueTypes`) REFERENCES `valuetypes` (`valueType`),
  CONSTRAINT `FK_user_fefine_variable_plc_rw_config` FOREIGN KEY (`plc_rw_area_did`) REFERENCES `plc_rw_config` (`plc_rw_area_did`),
  CONSTRAINT `FK_user_fefine_variable_variabletype` FOREIGN KEY (`variableTypeID`) REFERENCES `variabletype` (`variableTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=121 DEFAULT CHARSET=utf8 COMMENT='用户定义变量表。variableTypeID等于1表示为系统变量，系统变量与PLC变量，不是在同一个UI界面上显示的。\r\n系统变量不会与PLC地址绑定。非系统变量可能会与PLC地址绑定关联，用于接口数据上传。\r\n每个MES ID的变量关联的plc_rw_area_did都必须是Ｒ读类型的．MES ID的变量属于OUTPUT参数，上传给mes的．needMonitorLog都必须为１\r\nmes input参数变量也全部needMonitorLog必须是1';

-- Dumping data for table yh_slix.user_define_variable: ~48 rows (大约)
/*!40000 ALTER TABLE `user_define_variable` DISABLE KEYS */;
INSERT INTO `user_define_variable` (`variableID`, `variableName`, `variableTypeID`, `value`, `description`, `valueTypes`, `variableLength`, `plc_rw_area_did`, `plc_address`, `needMonitorLog`, `remark`, `datatime`) VALUES
	(1, 'AssetsNO', 1, 'SLIX011U', '设备编号', 'String', 1, NULL, NULL, b'0', '不可删除此', '2019-12-18 14:44:23'),
	(2, 'CCDVersion', 1, '3.02.6', 'CCD软件版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:29'),
	(3, 'ClearAlarmRecord', 1, '31', '历史报警记录保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:30'),
	(4, 'ClearHmiSettingMonitorLog', 1, '30', '重点监控参数保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:36'),
	(5, 'ClearInputDownloadHistory', 1, '180', 'Input下发历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:39'),
	(6, 'ClearInputUploadHistory', 1, '60', 'Output上载历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:40'),
	(7, 'ClearLog4net', 1, '30', '软件运行日志保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:41'),
	(8, 'ClearMESinterfaceLog', 1, '3', 'MES接口调用记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:41'),
	(9, 'ClearOperationLog', 1, '30', '操作日志保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:45'),
	(10, 'ClearPLCinteractiveLog', 1, '31', 'PC/PLC交互记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:48'),
	(11, 'ClearProductionData', 1, '30', '生产记录数据保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:44:50'),
	(12, 'dayShiftStartTime', 1, '7:30', '白班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(14, 'DefaultMesUrl', 1, 'https://www.baidu.com/', '默认MES网址', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:45:06'),
	(15, 'DeviceGroupDID', 1, 'MIB#1', '设备组别', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:46'),
	(19, 'selfCheckCycle', 1, '2', '设备自检周期(h)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-09-18 09:46:39'),
	(20, 'quick-wearPartCycle', 1, '60', '易损件检查周期(min)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:49'),
	(21, 'localIpAdr', 1, '10.217.99.224', '本机 IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-12-18 17:17:12'),
	(22, 'localTcpPortNo', 1, '60001', '本机tcp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-12-18 14:45:43'),
	(23, 'localUdpRecvPortNo', 1, '50020', '本机udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-12-18 17:25:38'),
	(24, 'localUdpSendPortNo', 1, '50002', '本机udp发送端口', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-12-18 14:45:47'),
	(25, 'MEScontacts', 1, '张某某150-1234-1234', 'MES联系人', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:45:49'),
	(26, 'Model', 1, 'M20', '生产型号', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:45:52'),
	(27, 'nightShiftStartTime', 1, '19:31', '夜班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:45:58'),
	(29, 'PCName', 1, 'ATL-PTF', 'PCName', 'String', 1, NULL, NULL, b'1', NULL, '2019-12-18 14:46:02'),
	(30, 'PEcontacts', 1, '李某150-1234-1234', 'PE联系人', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-18 14:46:06'),
	(31, 'serverIpAdr', 1, '172.23.11.66', 'MES服务端IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-12-18 17:17:41'),
	(32, 'serverUdpPortNo', 1, '50001', 'MES服务端udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-12-18 17:17:55'),
	(100, 'Code', 2, '0', 'PLC账户密码', 'String', 1, 2, 'D16078', b'0', NULL, '2020-09-18 16:30:40'),
	(101, 'Account', 2, '0', 'PLC账户', 'String', 1, 2, 'D16072', b'0', NULL, '2020-09-18 16:30:40'),
	(102, 'HmiPermissionRequest', 2, '0', 'PLC获取HMI权限触发', 'Int16', 1, 2, 'D16070', b'0', NULL, '2020-09-18 16:30:40'),
	(103, 'ParentEQState', 2, '0', '设备状态', 'Int16', 1, 3, 'D16090', b'0', NULL, '2020-09-18 16:30:40'),
	(104, 'AndonState', 2, '0', 'Andon状态', 'Int16', 1, 3, 'D16091', b'0', NULL, '2020-09-18 16:30:41'),
	(105, 'Quantity', 2, '0', '当班产量', 'Int16', 1, 3, 'D16092', b'0', NULL, '2020-09-18 16:30:40'),
	(106, 'PLCversion', 2, '0', 'PLC程序版本号', 'String', 1, 6, 'D16200', b'0', NULL, '2020-09-18 16:30:40'),
	(107, 'HMIversion', 2, '0', 'HMI版本', 'String', 1, 6, 'D16210', b'0', NULL, '2020-09-18 16:30:40'),
	(108, 'MesReply', 2, '0', 'MES响应状态', 'Int32', 1, 12, 'D18006', b'0', NULL, '2020-09-18 16:30:40'),
	(109, 'HeatBeat', 2, '0', '心跳', 'Float', 1, 12, 'D18000', b'0', NULL, '2020-09-18 16:30:40'),
	(110, 'UserLevel', 2, '0', 'PLC账户等级', 'Int16', 1, 12, 'D18004', b'0', NULL, '2020-09-18 16:30:40'),
	(111, 'MESstatusToPLC', 2, '0', 'MES连接状态toPLC', 'Float', 1, 12, 'D18002', b'0', NULL, '2020-09-18 16:30:40'),
	(112, 'A007Count', 2, '0', 'A007首件数量', 'Int16', 1, 13, 'D18052', b'0', NULL, '2020-09-18 16:30:41'),
	(113, 'ControlCode', 2, '0', 'mes控机操作的PLC地址', 'Int16', 1, 13, 'D18050', b'0', NULL, '2020-09-18 16:30:40'),
	(114, 'StateCode', 2, '0', 'A007首件信号', 'Int16', 1, 13, 'D18051', b'0', NULL, '2020-09-18 16:30:41'),
	(115, 'SoftVersion', 2, '0', 'ATL_MES软件版本号', 'String', 1, 22, 'D18240', b'0', NULL, '2020-09-18 16:30:41'),
	(116, 'AutoPopMonitorPage', 1, '1', '是否启用电脑无操作几分钟后自动弹出监控画面', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:40'),
	(117, 'LabChinese', 1, 'FEF', '工序名称(中文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:40'),
	(118, 'LabEnglish', 1, 'FEF', '工序名称(英文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:40'),
	(119, 'LabVersion', 1, 'FEF', '版本号', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:41'),
	(120, 'DefaultAcount', 1, 'superAdmin', 'Default Acount', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-18 16:30:41');
/*!40000 ALTER TABLE `user_define_variable` ENABLE KEYS */;

-- Dumping structure for table yh_slix.valuetypes
CREATE TABLE IF NOT EXISTS `valuetypes` (
  `valueType` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`valueType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='变量类型';

-- Dumping data for table yh_slix.valuetypes: ~8 rows (大约)
/*!40000 ALTER TABLE `valuetypes` DISABLE KEYS */;
INSERT INTO `valuetypes` (`valueType`, `remark`) VALUES
	('Bit', '字区里面的位，0或者1'),
	('Bool', 'M\\R区的位，0或者1'),
	('Double', '64位浮点数'),
	('Float', '32位浮点数'),
	('Int16', '16位整数'),
	('Int32', '32位整数'),
	('Int64', '64位整数'),
	('String', '字符串');
/*!40000 ALTER TABLE `valuetypes` ENABLE KEYS */;

-- Dumping structure for table yh_slix.variabletype
CREATE TABLE IF NOT EXISTS `variabletype` (
  `variableTypeID` int(11) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`variableTypeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='自定义变量的类型，不可更改此表的内容';

-- Dumping data for table yh_slix.variabletype: ~2 rows (大约)
/*!40000 ALTER TABLE `variabletype` DISABLE KEYS */;
INSERT INTO `variabletype` (`variableTypeID`, `remark`) VALUES
	(1, '系统变量(不读写PLC的变量)'),
	(2, '与PLC信号交互的变量(包括HMI监控变量)，一般为CIO、D');
/*!40000 ALTER TABLE `variabletype` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
