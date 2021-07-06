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


-- Dumping database structure for ptf
CREATE DATABASE IF NOT EXISTS `ptf` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `ptf`;

-- Dumping structure for table ptf.alarm_id_for_cut
CREATE TABLE IF NOT EXISTS `alarm_id_for_cut` (
  `did` int(11) NOT NULL,
  PRIMARY KEY (`did`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='禁止删除此表';

-- Dumping data for table ptf.alarm_id_for_cut: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_id_for_cut` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_id_for_cut` ENABLE KEYS */;

-- Dumping structure for table ptf.alarm_level
CREATE TABLE IF NOT EXISTS `alarm_level` (
  `alarm_level_id` int(11) NOT NULL AUTO_INCREMENT,
  `level_name` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`alarm_level_id`),
  UNIQUE KEY `level_name` (`level_name`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='报警等级';

-- Dumping data for table ptf.alarm_level: ~4 rows (大约)
/*!40000 ALTER TABLE `alarm_level` DISABLE KEYS */;
INSERT INTO `alarm_level` (`alarm_level_id`, `level_name`, `remark`) VALUES
	(1, 'A', '需要ME工程师处理'),
	(2, 'B', '需要现场操作员处理'),
	(3, 'C', '设备动作过程中的联锁及提示类信息'),
	(4, 'D', '备用');
/*!40000 ALTER TABLE `alarm_level` ENABLE KEYS */;

-- Dumping structure for table ptf.alarm_record
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
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COMMENT='报警记录';

-- Dumping data for table ptf.alarm_record: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_record` DISABLE KEYS */;
INSERT INTO `alarm_record` (`did`, `EquipmentID`, `UploadParamID`, `alarm_time`, `dispose_time`, `create_by`, `mhandler`, `duration`) VALUES
	(9, 'ACOA0022', 'Alarm018', '2020-08-18 14:56:06', '2020-08-18 15:02:31', NULL, NULL, 6.42),
	(10, 'ACOA0022', 'Alarm023', '2020-08-18 14:59:40', '2020-08-18 15:02:31', NULL, NULL, 2.85),
	(11, 'ACOA0022', 'Alarm015', '2020-08-18 14:59:40', '2020-08-18 15:02:31', NULL, NULL, 2.85);
/*!40000 ALTER TABLE `alarm_record` ENABLE KEYS */;

-- Dumping structure for event ptf.alarm_record_cutclip
DELIMITER //
CREATE DEFINER=`root`@`localhost` EVENT `alarm_record_cutclip` ON SCHEDULE EVERY 3 SECOND STARTS '2019-07-23 14:13:09' ENDS '2029-07-23 14:13:09' ON COMPLETION NOT PRESERVE ENABLE COMMENT '将报警信息从临时表剪切放入记录表' DO BEGIN
	truncate table alarm_id_for_cut;
	insert into alarm_id_for_cut (did) select alarm_temp_did from alarm_temp t
	where t.dispose_state = 1;
	insert into alarm_record (EquipmentID, UploadParamID, alarm_time, dispose_time, create_by, mhandler, duration) 
	(select t.EquipmentID, t.UploadParamID, t.alarm_time, t.dispose_time, t.create_by, t.mhandler, t.duration 
	from alarm_temp t 
	where t.alarm_temp_did in (select * from alarm_id_for_cut));
	delete from alarm_temp  where alarm_temp_did in (select c.did from alarm_id_for_cut c);
END//
DELIMITER ;

-- Dumping structure for table ptf.alarm_temp
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
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COMMENT='dispose_state = 0 表示暂未处理的报警\r\ndispose_state = 1 表示暂已处理的报警\r\nduration表示报警时长(min)';

-- Dumping data for table ptf.alarm_temp: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_temp` DISABLE KEYS */;
INSERT INTO `alarm_temp` (`alarm_temp_did`, `EquipmentID`, `UploadParamID`, `alarm_time`, `dispose_state`, `dispose_time`, `create_by`, `mhandler`, `duration`) VALUES
	(7, 'ACOA0022', 'Alarm002', '2020-08-18 15:02:31', 0, NULL, NULL, NULL, 0.00),
	(8, 'ACOA0022', 'Alarm007', '2020-08-18 15:02:31', 0, NULL, NULL, NULL, 0.00),
	(9, 'ACOA0022', 'Alarm015', '2020-08-18 15:22:40', 0, NULL, NULL, NULL, 0.00);
/*!40000 ALTER TABLE `alarm_temp` ENABLE KEYS */;

-- Dumping structure for table ptf.deivce_equipmentid
CREATE TABLE IF NOT EXISTS `deivce_equipmentid` (
  `EquipmentID` varchar(50) NOT NULL,
  `A007Jason` mediumtext DEFAULT NULL,
  `ControlCode` varchar(50) DEFAULT NULL COMMENT 'MES控机',
  `ParentEQState` varchar(50) DEFAULT NULL COMMENT '主设备状态代码',
  `AndonState` varchar(50) DEFAULT NULL COMMENT 'andon状态',
  `Quantity` varchar(50) DEFAULT NULL COMMENT '当班产量',
  `MESdisconnected` varchar(50) DEFAULT NULL COMMENT 'MES连接失败需要写入',
  `HeatBeat` varchar(50) DEFAULT NULL COMMENT '上位机写入心跳',
  `MesReply` varchar(50) DEFAULT NULL COMMENT '写入MES回复信息超时',
  `HmiPermissionRequest` varchar(50) DEFAULT NULL COMMENT 'PLC变量；PLC请求上位机读取帐号密码,等待MES下发权限''',
  `Account` varchar(50) DEFAULT NULL COMMENT 'PLC变量保存了PLC里的帐号',
  `Code` varchar(50) DEFAULT NULL COMMENT 'PLC变量保存了PLC里的密码',
  `UserLevel` varchar(50) DEFAULT NULL COMMENT 'MES下发给PLC的权限',
  `Count` varchar(50) DEFAULT NULL COMMENT 'A007下发的count信息保存',
  `ProductMode` varchar(50) DEFAULT NULL COMMENT '0为量产，1为首件。只写一次',
  `SpartLimitControl` varchar(50) DEFAULT NULL COMMENT 'INT16',
  `InputLimitControl` varchar(50) DEFAULT NULL COMMENT 'INT16',
  `Model` varchar(50) DEFAULT NULL COMMENT 'A007下发的Model数据或者为Andon文件夹路径',
  PRIMARY KEY (`EquipmentID`),
  KEY `FK_deivce_equipmentid_plc_1` (`ControlCode`),
  KEY `FK_deivce_equipmentid_plc_2` (`ParentEQState`),
  KEY `FK_deivce_equipmentid_plc_3` (`AndonState`),
  KEY `FK_deivce_equipmentid_plc_4` (`MESdisconnected`),
  KEY `FK_deivce_equipmentid_plc_5` (`HeatBeat`),
  KEY `FK_deivce_equipmentid_plc_6` (`MesReply`),
  KEY `FK_deivce_equipmentid_plc_7` (`HmiPermissionRequest`),
  KEY `FK_deivce_equipmentid_plc_8` (`Account`),
  KEY `FK_deivce_equipmentid_plc_9` (`Code`),
  KEY `FK_deivce_equipmentid_plc_10` (`UserLevel`),
  KEY `FK_deivce_equipmentid_plc_11` (`Quantity`),
  KEY `FK_deivce_equipmentid_plc_12` (`Count`),
  KEY `FK_deivce_equipmentid_plc_13` (`ProductMode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='ControlCode用于A007或者A011接口MES指令控制设备，对于PLC设备，该值为寄存器的地址。参照device_controlcode表来写数字到PLC里面。\r\nParentEQState用于A019接口上传主设备状态代码。ParentEQState表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nAndonState用于A019接口上传Andon状态代码。AndonState表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nQuantity用于A019接口上传当班产量。Quantity表示寄存器地址，或者系统变量名。读取到PLC值后，上传。\r\nHeatBeat只针对PLC设备\r\nMesReply用来写入MES超时与否的信息\r\ndevice_controlcode表里配置了：\r\nHeatBeat\r\nMESconnected\r\nMESdisconnected\r\nMesReplyNG\r\nMesReplyOK\r\ndevice_controlcode \r\n对应的PLC值的信息\r\nUserLevel 里写的值206、307、408分别表示操作、维修、开发三级权限\r\nStateCode对应的StateCode等于1表示做首件，否则等于0；Count对应的Count表示首件数量\r\n\r\n';

-- Dumping data for table ptf.deivce_equipmentid: ~0 rows (大约)
/*!40000 ALTER TABLE `deivce_equipmentid` DISABLE KEYS */;
/*!40000 ALTER TABLE `deivce_equipmentid` ENABLE KEYS */;

-- Dumping structure for table ptf.deivce_equipmentid_plc
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

-- Dumping data for table ptf.deivce_equipmentid_plc: ~0 rows (大约)
/*!40000 ALTER TABLE `deivce_equipmentid_plc` DISABLE KEYS */;
INSERT INTO `deivce_equipmentid_plc` (`EquipmentID`, `plcID`, `A007Jason`, `ControlCodeAddress`, `ParentEQStateAddress`, `AndonStateAddress`, `QuantityAddress`, `MESdisconnectedAddress`, `HeatBeatAddress`, `MesReplyAddress`, `HmiPermissionRequestAddress`, `AccountAddress`, `CodeAddress`, `UserLevelAddress`, `CountAddress`, `ProductModeAddress`, `SpartLimitControl`, `InputLimitControl`, `ModelAddress`, `SoftVersionAddress`) VALUES
	('ACOA0022', 1, '{"Header": {"SessionID": "662EA867-89EF-4BF2-9A58-653999D73F70","FunctionID": "A007","PCName": "1111","EQCode": "FEF001","SoftName": "ServerSoft","RequestTime": "2019-11-28 13:53:02 043"},"RequestInfo": {"Count": "5","CmdInfo": {"ControlCode": "Run","StateCode": "1","StateDesc": "123"},"UserInfo": {"UserID": "123","UserName": "ATL","UserLevel": "1"},"ResourceInfo": {"ResourceID": "EQ00000001","ResourceShift": "M"},"SpartInfo": [{"SpartName": "从机冷压轧辊","SpartID": "COLX-CZG0-7","SpartExpectedLifetime": "100","ChangeFlag": "true"},{"SpartName": "主机冷压轧辊","SpartID": "COLX-ZZG0-7","SpartExpectedLifetime": "100","ChangeFlag": "true"}],"ModelInfo": "123","ParameterInfo": [{"ParamID": "001","StandardValue": "45","UpperLimitValue": "50","LowerLimitValue": "30","Description": "从机传动侧主缸压力"}]}}', 'ControlCode', 'ParentEQState', 'AndonState', 'Quantity', 'MESstatusToPLC', 'HeatBeat', 'MesReply', 'HmiPermissionRequest', 'Account', 'Code', 'UserLevel', 'A007Count', 'StateCode', NULL, NULL, NULL, NULL);
/*!40000 ALTER TABLE `deivce_equipmentid_plc` ENABLE KEYS */;

-- Dumping structure for table ptf.device_alert
CREATE TABLE IF NOT EXISTS `device_alert` (
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `UploadParamID` varchar(50) NOT NULL COMMENT '报警编码',
  `ParamName` varchar(50) NOT NULL COMMENT '报警信息',
  `AlertLevel` varchar(20) NOT NULL COMMENT '报警等级',
  `IsAlarming` int(10) DEFAULT NULL COMMENT '当前是否为报警状态',
  `DataTime` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`EquipmentID`,`UploadParamID`),
  CONSTRAINT `FK_device_alert_config_deivce_equipmentid` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid` (`EquipmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.device_alert: ~0 rows (大约)
/*!40000 ALTER TABLE `device_alert` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_alert` ENABLE KEYS */;

-- Dumping structure for table ptf.device_alert_config
CREATE TABLE IF NOT EXISTS `device_alert_config` (
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `plcID` int(11) DEFAULT NULL,
  `UploadParamID` varchar(50) NOT NULL COMMENT '报警编码',
  `ParamName` varchar(50) NOT NULL COMMENT '报警信息',
  `AlertLevel` varchar(20) NOT NULL COMMENT '报警等级',
  `AlertBitAddr` varchar(50) DEFAULT NULL COMMENT '报警地址',
  `DataTime` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`EquipmentID`,`UploadParamID`),
  CONSTRAINT `FK_device_alert_config_deivce_equipmentid_plc` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid_plc` (`EquipmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='设备报警配置\r\nisAlarming字段适用于板卡设备，报警信息并非从PLC中获取得到。对于板卡设备，AlertBitAddr字段无意义。';

-- Dumping data for table ptf.device_alert_config: ~24 rows (大约)
/*!40000 ALTER TABLE `device_alert_config` DISABLE KEYS */;
INSERT INTO `device_alert_config` (`EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `AlertLevel`, `AlertBitAddr`, `DataTime`) VALUES
	('ACOA0022', 1, 'Alarm001', '上料输送带电芯追尾(R13/R14），请检查！', 'A', 'D1000.0', '2020-08-18 14:34:59'),
	('ACOA0022', 1, 'Alarm002', '备用61001', 'A', 'D1000.1', '2020-08-18 14:35:13'),
	('ACOA0022', 1, 'Alarm003', '左切边防切爆感应异常(R203）,请取走电芯并清除该工位电芯记忆！', 'A', 'D1000.2', '2020-08-18 14:35:28'),
	('ACOA0022', 1, 'Alarm004', '左切刀使用寿命已到,请更换切刀！', 'A', 'D1000.3', '2020-08-18 14:35:32'),
	('ACOA0022', 1, 'Alarm005', '左折角光纤检测异常(R203),请取走电芯并清除该工位电芯记忆！', 'A', 'D1000.4', '2020-08-18 14:35:34'),
	('ACOA0022', 1, 'Alarm006', '左滴胶感应异常(R300),请取走电芯左精整位并清除该工位电芯记忆！', 'A', 'D1000.5', '2020-08-18 14:35:37'),
	('ACOA0022', 1, 'Alarm007', '左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！', 'A', 'D1000.6', '2020-08-18 14:35:39'),
	('ACOA0022', 1, 'Alarm008', '换向锁紧防呆感应异常(R406),请检查左冷烫位夹具是否合上！', 'A', 'D1000.7', '2020-08-18 14:35:41'),
	('ACOA0022', 1, 'Alarm009', '右切边光纤感应异常(R800),请取走电芯并清除该工位电芯记忆！', 'A', 'D1000.8', '2020-08-18 14:38:25'),
	('ACOA0022', 1, 'Alarm010', '右切刀使用寿命已到,请更换切刀！', 'A', 'D1000.9', '2020-08-18 14:38:27'),
	('ACOA0022', 1, 'Alarm011', '右折角感应异常(R803),请取走电芯并清除该工位电芯记忆！', 'A', 'D1000.10', '2020-08-18 14:38:30'),
	('ACOA0022', 1, 'Alarm012', '右滴胶感应异常(R814),请取走电芯右精整位并清除该工位电芯记忆！', 'A', 'D1000.11', '2020-08-18 14:38:32'),
	('ACOA0022', 1, 'Alarm013', '右换胶时间已到,并切换到左滴胶操作页面清零计时！！', 'A', 'D1000.12', '2020-08-18 14:38:35'),
	('ACOA0022', 1, 'Alarm014', '下料推电芯感应异常（R1002），请手动纠正电芯位置！', 'A', 'D1000.13', '2020-08-18 15:13:10'),
	('ACOA0022', 1, 'Alarm015', '下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！', 'A', 'D1000.14', '2020-08-18 15:13:12'),
	('ACOA0022', 1, 'Alarm016', '连续3个扫码不良报警', 'A', 'D1000.15', '2020-08-18 15:13:13'),
	('ACOA0022', 1, 'Alarm017', '上料翻转位有电池（R101），请手动取走并关闭真空阀！', 'A', 'D1001.0', '2020-08-18 14:38:54'),
	('ACOA0022', 1, 'Alarm018', '上料搬运A位有电池（R112），请手动取走并关闭真空阀！', 'A', 'D1001.1', '2020-08-18 14:39:06'),
	('ACOA0022', 1, 'Alarm019', '上料搬运B位有电池（R113），请手动取走并关闭真空阀！', 'A', 'D1001.2', '2020-08-18 14:39:08'),
	('ACOA0022', 1, 'Alarm020', '备用61103', 'A', 'D1001.3', '2020-08-18 14:39:10'),
	('ACOA0022', 1, 'Alarm021', '备用61104', 'A', 'D1001.4', '2020-08-18 14:39:12'),
	('ACOA0022', 1, 'Alarm022', '备用61105', 'A', 'D1001.5', '2020-08-18 14:39:14'),
	('ACOA0022', 1, 'Alarm023', '备用61106', 'A', 'D1001.6', '2020-08-18 14:39:16'),
	('ACOA0022', 1, 'Alarm024', '备用61107', 'A', 'D1001.7', '2020-08-18 14:39:20');
/*!40000 ALTER TABLE `device_alert_config` ENABLE KEYS */;

-- Dumping structure for table ptf.device_childeqcode
CREATE TABLE IF NOT EXISTS `device_childeqcode` (
  `ChildEQCode` varchar(50) NOT NULL COMMENT '填写内容：工位code',
  `ChildEQStateAddress` varchar(50) DEFAULT NULL,
  `Remark` varchar(50) NOT NULL,
  PRIMARY KEY (`ChildEQCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='一个Baking设备，有多个PLC，没有ChildEQ,一个PLC对应多个EquipmentID\r\nDegassing、注液机设备，只有一个PLC，多个ChildEQ\r\n其他PLC设备，都是只有一个EquipmentID，无ChildEQ，PLC数量可能多个\r\nChildEQStateAddress可以为系统变量，或者PLC变量，但是必须处于R类型的地址段\r\nChildEQState为enable或者disable\r\n';

-- Dumping data for table ptf.device_childeqcode: ~0 rows (大约)
/*!40000 ALTER TABLE `device_childeqcode` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_childeqcode` ENABLE KEYS */;

-- Dumping structure for table ptf.device_controlcode
CREATE TABLE IF NOT EXISTS `device_controlcode` (
  `ControlCode` varchar(50) NOT NULL,
  `StateCode` varchar(50) NOT NULL,
  `describe` varchar(200) NOT NULL,
  `plcValue` int(11) NOT NULL,
  PRIMARY KEY (`ControlCode`,`StateCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='A007或者A011\r\nMES控机指令表\r\nplcValue：要写入的值';

-- Dumping data for table ptf.device_controlcode: ~29 rows (大约)
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
	('HeatBeat', '', '写入PLC的心跳值', 99),
	('HOLD_ENG(Suspend)', 'HOLD', '由MES 下达指令给到设备，设备暂停加工，不允许新的物料进入设备中加工', 8),
	('HOLD_ENG(Suspend)', 'WAIT_ENG', '生产部发现设备有问题，将设备交由工程师处理', 9),
	('IDLE', 'IDLE', '设备空闲', 3),
	('IDLE', 'Switch_Shift', '设备正常，由于需要换班或午休', 5),
	('IDLE', 'WAIT_Material', '设备正常，因为无料生产等候', 4),
	('IDLE', 'WAIT_PRD', '设备工程师修完机之后交付给生产部', 6),
	('InputLimitControl', 'SET', 'Input总控，下发Input成功后输出总控开关值', 1),
	('MESconnected', '', 'MES连接成功', 10),
	('MESdisconnected', '', 'MES连接失败', 20),
	('MesReplyNG', '', '当3次未回复指令为A015、A013、A035、A055、A033、A029、A031、A051时需要向设备发出停机指令，并启动设备的有声报警信号提示MES发生异常需要介入处理', 1),
	('MesReplyOK', '', '非MesReplyNG状态', 2),
	('PM', '', '设备做PM', 7),
	('RUN', '', '设备正在正常加工产品', 1),
	('SingleInputLimitControl', 'Disable', 'Input单控，MES不需要下发Input的开关值', 10),
	('SingleInputLimitControl', 'Enable', 'Input单控，MES需要下发Input的开关值', 0),
	('SingleSpartLimitControl', 'Disable', '易损件单控，不需要下发易损件开关值', 0),
	('SingleSpartLimitControl', 'Enable', '易损件单控，需要下发易损件开关值', 1),
	('SpartLimitControl', 'SET', '易损件总控，下发易损件信息成功后输出总控开关值', 1),
	('STOP', '', 'MES要求停机', 22),
	('TEST', '', ' 测机、首件加工时设置MES 为Test；要保证完成首件任务后不能投入批量生产', 2);
/*!40000 ALTER TABLE `device_controlcode` ENABLE KEYS */;

-- Dumping structure for table ptf.device_inputoutput
CREATE TABLE IF NOT EXISTS `device_inputoutput` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `SendParamID` varchar(50) NOT NULL COMMENT 'input参数ID-A007',
  `UploadParamID` varchar(50) NOT NULL COMMENT 'output参数ID-A013、监控值用于A045、A017点检',
  `ParamName` varchar(50) NOT NULL COMMENT '参数名称',
  `Type` varchar(20) NOT NULL COMMENT '参数类型 float',
  `SettingValue` varchar(50) NOT NULL COMMENT '设定值-A007',
  `UpperLimitValue` varchar(50) NOT NULL COMMENT '设定上限-A007',
  `LowerLimitValue` varchar(50) NOT NULL COMMENT '设定下限-A007',
  `LimitControl` varchar(50) NOT NULL COMMENT '参数是否启用控制',
  `InputChangeMonitor` varchar(50) NOT NULL COMMENT '设定参数变更监控-A045；Input参数下发界面的当前值监控',
  `ActualValue` varchar(50) NOT NULL COMMENT '实际值-A017点检',
  `BycellOutput` varchar(50) NOT NULL COMMENT 'by产品的过程值-A013',
  `ParamValueRatio` float NOT NULL COMMENT '参数倍率',
  PRIMARY KEY (`ID`),
  KEY `EquipmentID` (`EquipmentID`),
  KEY `SendParamID` (`SendParamID`),
  KEY `UploadParamID` (`UploadParamID`),
  KEY `FK_value_types` (`Type`),
  CONSTRAINT `FK_device_in_out_put_mes_config_deivce_equipmentid` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid` (`EquipmentID`),
  CONSTRAINT `FK_value_types2` FOREIGN KEY (`Type`) REFERENCES `valuetypes` (`valueType`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.device_inputoutput: ~0 rows (大约)
/*!40000 ALTER TABLE `device_inputoutput` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_inputoutput` ENABLE KEYS */;

-- Dumping structure for table ptf.device_inputoutput_config
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

-- Dumping data for table ptf.device_inputoutput_config: ~44 rows (大约)
/*!40000 ALTER TABLE `device_inputoutput_config` DISABLE KEYS */;
INSERT INTO `device_inputoutput_config` (`ID`, `EquipmentID`, `plcID`, `SendParamID`, `UploadParamID`, `ParamName`, `Type`, `SettingValueAddr`, `UpperLimitValueAddr`, `LowerLimitValueAddr`, `LimitControl`, `InputChangeMonitorAddr`, `ActualValueAddr`, `BycellOutputAddr`, `ParamValueRatio`) VALUES
	(1, 'ACOA0022', 1, '001', '50819', '左滴胶枪温度(℃)', 'Int16', 'D1050', 'D1060', 'D1070', 'D1028', 'D1040', 'D1080', '', 1),
	(2, 'ACOA0022', 1, '001', '50820', '右滴胶枪温度(℃)', 'Int16', 'D1052', 'D1062', 'D1072', 'D1029', 'D1042', 'D1082', '', 1),
	(3, 'ACOA0022', 1, '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', 'D1054', 'D1064', 'D1074', 'D1030', 'D1044', 'D1084', '', 1),
	(4, 'ACOA0022', 1, '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', 'D1056', 'D1066', 'D1076', 'D1031', 'D1046', 'D1086', '', 1),
	(5, 'ACOA0022', 1, '130515', '50952', '右一折热压上烫头温度(℃)', 'Int16', 'D1058', 'D1068', 'D1078', 'D1032', '', 'D1088', '', 1);
/*!40000 ALTER TABLE `device_inputoutput_config` ENABLE KEYS */;

-- Dumping structure for table ptf.device_spart
CREATE TABLE IF NOT EXISTS `device_spart` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `UploadParamID` varchar(50) NOT NULL COMMENT '关键配件ID',
  `ParamName` varchar(50) NOT NULL COMMENT '关键配件名称',
  `Type` varchar(20) NOT NULL COMMENT '参数类型 Float,Int32',
  `SpartExpectedLifetime` int(11) NOT NULL COMMENT '关键配件预期寿命-A007下发值',
  `MesSettingUsedLife` float NOT NULL COMMENT 'MES下发当前使用寿命',
  `ReadUsedLife` float NOT NULL COMMENT '从PLC里读取到的当前使用寿命',
  `LimitControl` varchar(50) DEFAULT NULL COMMENT '分开关',
  `PercentWarning` float(11,7) NOT NULL COMMENT '预警阈值',
  `ParamValueRatio` float NOT NULL COMMENT '倍率',
  `ChangeDate` datetime NOT NULL DEFAULT current_timestamp(),
  `ChangeUser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `EquipmentID_UploadParamID` (`EquipmentID`,`UploadParamID`),
  KEY `FK` (`Type`),
  CONSTRAINT `FK1` FOREIGN KEY (`Type`) REFERENCES `valuetypes` (`valueType`),
  CONSTRAINT `FK_device_spart_config_deivce_equipmentid` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid` (`EquipmentID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.device_spart: ~0 rows (大约)
/*!40000 ALTER TABLE `device_spart` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_spart` ENABLE KEYS */;

-- Dumping structure for table ptf.device_spart_config
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

-- Dumping data for table ptf.device_spart_config: ~10 rows (大约)
/*!40000 ALTER TABLE `device_spart_config` DISABLE KEYS */;
INSERT INTO `device_spart_config` (`id`, `EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `MesSettingUsedLife`, `NeedDownLoadSpartExpectedLifetimeToPLC`, `SettingValueAddr`, `SettingActualValueAddr`, `ActualValueAddr`, `LimitControl`, `ParamValueRatio`, `PercentWarning`, `ChangeDate`, `ChangeUser`) VALUES
	(2, 'ACOA0022', 1, 'temp2', '关键配件预期寿命2', 'Float', 100, 0, 0, 'D1016', 'D1022', 'D1010', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(3, 'ACOA0022', 1, 'temp3', '关键配件预期寿命3', 'Float', 100, 0, 0, 'D1018', 'D1024', 'D1012', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(4, 'ACOA0022', 1, 'temp4', '关键配件预期寿命4', 'Float', 100, 0, 0, 'D1020', 'D1026', 'D1014', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin');
/*!40000 ALTER TABLE `device_spart_config` ENABLE KEYS */;

-- Dumping structure for table ptf.device_status_code
CREATE TABLE IF NOT EXISTS `device_status_code` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `key` varchar(50) NOT NULL,
  `status` varchar(50) NOT NULL COMMENT '状态',
  `plcValue` int(11) NOT NULL COMMENT '从PLC里读到的值',
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `key` (`key`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COMMENT='上位机上传PLC里寄存器里的状态值给MES';

-- Dumping data for table ptf.device_status_code: ~7 rows (大约)
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

-- Dumping structure for table ptf.device_tag
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

-- Dumping data for table ptf.device_tag: ~0 rows (大约)
/*!40000 ALTER TABLE `device_tag` DISABLE KEYS */;
/*!40000 ALTER TABLE `device_tag` ENABLE KEYS */;

-- Dumping structure for table ptf.facility_info
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

-- Dumping data for table ptf.facility_info: ~0 rows (大约)
/*!40000 ALTER TABLE `facility_info` DISABLE KEYS */;
/*!40000 ALTER TABLE `facility_info` ENABLE KEYS */;

-- Dumping structure for table ptf.hmi_setting_monitor_log
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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COMMENT='用户自定义变量里重要参数记录，数据有变化的时候才记录';

-- Dumping data for table ptf.hmi_setting_monitor_log: ~0 rows (大约)
/*!40000 ALTER TABLE `hmi_setting_monitor_log` DISABLE KEYS */;
INSERT INTO `hmi_setting_monitor_log` (`logID`, `variableID`, `variableName`, `address`, `description`, `plc_ID`, `variableTypeID`, `value`, `logDateTime`) VALUES
	(2, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '1', '2020-08-18 15:51:34'),
	(3, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 15:56:18'),
	(4, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 16:05:45'),
	(5, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 16:09:14'),
	(6, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 16:48:17'),
	(7, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 16:56:34'),
	(8, 72, 'HMIversion', 'D35', 'HMI版本', 1, 2, '123', '2020-08-18 17:01:19');
/*!40000 ALTER TABLE `hmi_setting_monitor_log` ENABLE KEYS */;

-- Dumping structure for table ptf.input_log_from
CREATE TABLE IF NOT EXISTS `input_log_from` (
  `source` varchar(50) NOT NULL,
  PRIMARY KEY (`source`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='input参数来源';

-- Dumping data for table ptf.input_log_from: ~2 rows (大约)
/*!40000 ALTER TABLE `input_log_from` DISABLE KEYS */;
INSERT INTO `input_log_from` (`source`) VALUES
	('download'),
	('upload');
/*!40000 ALTER TABLE `input_log_from` ENABLE KEYS */;

-- Dumping structure for table ptf.input_variable_history
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
) ENGINE=InnoDB AUTO_INCREMENT=594 DEFAULT CHARSET=utf8 COMMENT='input参数下发历史数据以及手动上传PLC的历史数据\r\nEQID为设备名称：Baking设备必填字段，因为每个炉子可能做不一样的Model,每个炉子下发的时间点不一样。\r\nDataTime表示从PLC上传的时间或者下发到PLC的时间\r\nmodel字段表示当前生产的型号\r\ninput_variableDID表示input_variable表里的主键ID\r\n则需要在user_fefine_variable表里建立20个控制温度变量，个对应1个炉子，每个控制温度变量绑定一个PLC。';

-- Dumping data for table ptf.input_variable_history: ~0 rows (大约)
/*!40000 ALTER TABLE `input_variable_history` DISABLE KEYS */;
INSERT INTO `input_variable_history` (`ID`, `EquipmentID`, `SendParamID`, `UploadParamID`, `ParamName`, `Type`, `ParamValueRatio`, `Model`, `HistoryMaxValue`, `HistoryMinValue`, `HistoryStandardValue`, `ChangeMonitorValue`, `ActualValue`, `BycellOutputValue`, `DataTime`, `LogFrom`, `DownloadRemark`) VALUES
	(530, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 14:54:25', 'upload', 'download'),
	(531, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 14:54:25', 'upload', 'download'),
	(532, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 14:54:25', 'upload', 'download'),
	(533, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 14:54:25', 'upload', 'download'),
	(534, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:31', 'upload', 'download'),
	(535, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:31', 'upload', 'download'),
	(536, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:31', 'upload', 'download'),
	(537, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:31', 'upload', 'download'),
	(538, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:44', 'upload', 'download'),
	(539, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:44', 'upload', 'download'),
	(540, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:44', 'upload', 'download'),
	(541, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:02:44', 'upload', 'download'),
	(542, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:22:42', 'upload', 'download'),
	(543, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:22:42', 'upload', 'download'),
	(544, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:22:42', 'upload', 'download'),
	(545, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:22:42', 'upload', 'download'),
	(546, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '0', '0', '', '2020-08-18 15:23:14', 'download', ''),
	(547, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '0', '0', '', '2020-08-18 15:23:14', 'download', ''),
	(548, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:24:10', 'download', ''),
	(549, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:24:10', 'download', ''),
	(550, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:24:27', 'download', ''),
	(551, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:24:27', 'download', ''),
	(552, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:25:01', 'download', ''),
	(553, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '123', '50', '30', '45', '', '', '', '2020-08-18 15:25:01', 'download', ''),
	(554, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:34:41', 'upload', 'download'),
	(555, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:34:41', 'upload', 'download'),
	(556, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:34:41', 'upload', 'download'),
	(557, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:34:41', 'upload', 'download'),
	(558, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:51:30', 'upload', 'download'),
	(559, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:51:30', 'upload', 'download'),
	(560, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:51:30', 'upload', 'download'),
	(561, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:51:30', 'upload', 'download'),
	(562, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:56:10', 'upload', 'download'),
	(563, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:56:10', 'upload', 'download'),
	(564, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:56:10', 'upload', 'download'),
	(565, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', '1', '', '', '', '', '0', '', '', '2020-08-18 15:56:10', 'upload', 'download'),
	(566, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:05:33', 'upload', 'download'),
	(567, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:05:33', 'upload', 'download'),
	(568, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:05:33', 'upload', 'download'),
	(569, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:05:33', 'upload', 'download'),
	(570, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '123', '0', '0', '0', '0', '0', '', '2020-08-18 16:05:57', 'upload', ''),
	(571, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '123', '0', '0', '0', '0', '0', '', '2020-08-18 16:05:57', 'upload', ''),
	(572, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '123', '50', '30', '45', '0', '0', '', '2020-08-18 16:06:01', 'download', ''),
	(573, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '123', '50', '30', '45', '0', '0', '', '2020-08-18 16:06:01', 'download', ''),
	(574, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:09:10', 'upload', 'download'),
	(575, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:09:10', 'upload', 'download'),
	(576, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:09:10', 'upload', 'download'),
	(577, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:09:10', 'upload', 'download'),
	(578, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:31:19', 'upload', 'download'),
	(579, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:31:19', 'upload', 'download'),
	(580, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:31:19', 'upload', 'download'),
	(581, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:31:19', 'upload', 'download'),
	(582, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:48:13', 'upload', 'download'),
	(583, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:48:13', 'upload', 'download'),
	(584, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:48:13', 'upload', 'download'),
	(585, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:48:13', 'upload', 'download'),
	(586, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:56:31', 'upload', 'download'),
	(587, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:56:32', 'upload', 'download'),
	(588, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:56:32', 'upload', 'download'),
	(589, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 16:56:32', 'upload', 'download'),
	(590, 'ACOA0022', '001', '50819', '左滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 17:01:16', 'upload', 'download'),
	(591, 'ACOA0022', '001', '50820', '右滴胶枪温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 17:01:16', 'upload', 'download'),
	(592, 'ACOA0022', '130513', '50950', '左一折热压上烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 17:01:16', 'upload', 'download'),
	(593, 'ACOA0022', '130514', '50951', '左一折热压下烫头温度(℃)', 'Int16', '1', '', '', '', '', '0', '', '', '2020-08-18 17:01:16', 'upload', 'download');
/*!40000 ALTER TABLE `input_variable_history` ENABLE KEYS */;

-- Dumping structure for table ptf.log4net
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
) ENGINE=InnoDB AUTO_INCREMENT=3351 DEFAULT CHARSET=utf8 COMMENT='软件运行日志\r\nloglevel=INFO日志是会在在UI界面上显示查看的\r\nloglevel=DEBUG日志只是在txt格式的log文件里查看到\r\nloglevel=ERROR表示代码逻辑异常的日志\r\nloglevel=WARN表示警告提醒的日志信息\r\nloglevel=FATAl表示软件try catch到的异常信息';

-- Dumping data for table ptf.log4net: ~368 rows (大约)
/*!40000 ALTER TABLE `log4net` DISABLE KEYS */;
INSERT INTO `log4net` (`id`, `logdate`, `loglevel`, `logger`, `message`, `exception`, `softwareName`) VALUES
	(2554, '2020-08-17 21:57:26.392', 'INFO', '', '软件启动', '', 'PTF'),
	(2555, '2020-08-17 21:57:28.117', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2556, '2020-08-17 21:57:28.195', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2557, '2020-08-17 21:57:28.304', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2558, '2020-08-17 21:57:28.351', 'Error', '', '未将对象引用设置到对象的实例。', '', 'PTF'),
	(2559, '2020-08-17 21:57:32.102', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2560, '2020-08-17 21:57:32.227', 'INFO', '', '08-17 21:57:32 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2561, '2020-08-17 21:57:36.790', 'Error', '', '未将对象引用设置到对象的实例。', '', 'PTF'),
	(2562, '2020-08-17 21:57:39.243', 'Error', '', '未将对象引用设置到对象的实例。', '', 'PTF'),
	(2563, '2020-08-17 21:57:41.697', 'Error', '', '未将对象引用设置到对象的实例。', '', 'PTF'),
	(2564, '2020-08-17 21:57:44.322', 'Error', '', '未将对象引用设置到对象的实例。', '', 'PTF'),
	(2565, '2020-08-17 21:58:32.625', 'INFO', '', '软件启动', '', 'PTF'),
	(2566, '2020-08-17 21:58:33.953', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2567, '2020-08-17 21:58:34.000', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2568, '2020-08-17 21:58:34.110', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2569, '2020-08-17 21:59:10.089', 'INFO', '', '软件启动', '', 'PTF'),
	(2570, '2020-08-17 21:59:11.404', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2571, '2020-08-17 21:59:11.482', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2572, '2020-08-17 21:59:11.591', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2573, '2020-08-17 22:00:36.385', 'INFO', '', '软件启动', '', 'PTF'),
	(2574, '2020-08-17 22:00:37.614', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2575, '2020-08-17 22:00:37.864', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2576, '2020-08-17 22:00:43.536', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2577, '2020-08-17 22:00:51.792', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(2578, '2020-08-17 22:01:26.616', 'INFO', '', '软件启动', '', 'PTF'),
	(2579, '2020-08-17 22:01:27.773', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2580, '2020-08-17 22:01:27.836', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2581, '2020-08-17 22:01:27.929', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2582, '2020-08-17 22:16:10.723', 'INFO', '', '软件启动', '', 'PTF'),
	(2583, '2020-08-17 22:16:11.928', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 D:项目文件夹MES 2.0框架代码MTW20200727ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2584, '2020-08-17 22:16:12.022', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2585, '2020-08-17 22:16:12.100', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2586, '2020-08-17 22:16:18.662', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2587, '2020-08-17 22:16:18.787', 'INFO', '', '08-17 22:16:18 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2588, '2020-08-17 22:16:19.584', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2589, '2020-08-17 22:16:19.599', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2590, '2020-08-18 11:33:20.879', 'INFO', '', '软件启动', '', 'PTF'),
	(2591, '2020-08-18 11:33:22.110', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2592, '2020-08-18 11:33:22.188', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2593, '2020-08-18 11:33:26.084', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2594, '2020-08-18 11:33:26.206', 'INFO', '', '08-18 11:33:26 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2595, '2020-08-18 11:33:41.044', 'INFO', '', '系统配置错误', '', 'PTF'),
	(2596, '2020-08-18 11:36:32.724', 'INFO', '', '软件启动', '', 'PTF'),
	(2597, '2020-08-18 11:36:33.989', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2598, '2020-08-18 11:36:34.056', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2599, '2020-08-18 11:36:37.960', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2600, '2020-08-18 11:36:38.069', 'INFO', '', '08-18 11:36:38 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2601, '2020-08-18 11:36:42.896', 'INFO', '', '系统配置错误', '', 'PTF'),
	(2602, '2020-08-18 11:37:02.706', 'INFO', '', '软件启动', '', 'PTF'),
	(2603, '2020-08-18 11:37:03.879', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2604, '2020-08-18 11:37:04.078', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2605, '2020-08-18 11:38:24.380', 'INFO', '', '软件启动', '', 'PTF'),
	(2606, '2020-08-18 11:38:25.598', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2607, '2020-08-18 11:38:25.660', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2608, '2020-08-18 11:38:25.805', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2609, '2020-08-18 11:38:29.607', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2610, '2020-08-18 11:38:29.741', 'INFO', '', '08-18 11:38:29 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2611, '2020-08-18 11:38:34.823', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2612, '2020-08-18 11:38:34.840', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2613, '2020-08-18 11:38:56.024', 'INFO', '', '软件启动', '', 'PTF'),
	(2614, '2020-08-18 11:38:57.160', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2615, '2020-08-18 11:38:57.225', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2616, '2020-08-18 11:38:57.360', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2617, '2020-08-18 11:39:01.120', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2618, '2020-08-18 11:39:01.255', 'INFO', '', '08-18 11:39:01 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2619, '2020-08-18 13:09:29.581', 'INFO', '', '软件启动', '', 'PTF'),
	(2620, '2020-08-18 13:09:30.742', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2621, '2020-08-18 13:09:30.762', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2622, '2020-08-18 13:09:30.939', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2623, '2020-08-18 13:09:33.264', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(2624, '2020-08-18 13:09:34.699', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2625, '2020-08-18 13:09:34.807', 'INFO', '', '08-18 13:09:34 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2626, '2020-08-18 13:09:35.677', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2627, '2020-08-18 13:09:35.717', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2628, '2020-08-18 13:09:57.045', 'INFO', '', '软件启动', '', 'PTF'),
	(2629, '2020-08-18 13:09:58.170', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2630, '2020-08-18 13:09:58.232', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2631, '2020-08-18 13:09:58.386', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2632, '2020-08-18 13:10:02.140', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2633, '2020-08-18 13:10:02.272', 'INFO', '', '08-18 13:10:02 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2634, '2020-08-18 13:10:02.622', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2635, '2020-08-18 13:10:02.664', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2636, '2020-08-18 13:10:44.450', 'INFO', '', '软件启动', '', 'PTF'),
	(2637, '2020-08-18 13:10:45.678', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2638, '2020-08-18 13:10:45.744', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2639, '2020-08-18 13:10:45.869', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2640, '2020-08-18 13:10:49.646', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2641, '2020-08-18 13:10:49.782', 'INFO', '', '08-18 13:10:49 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2642, '2020-08-18 13:10:51.622', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2643, '2020-08-18 13:10:51.634', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2644, '2020-08-18 13:11:22.089', 'INFO', '', '软件启动', '', 'PTF'),
	(2645, '2020-08-18 13:11:23.292', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2646, '2020-08-18 13:11:23.371', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2647, '2020-08-18 13:11:23.492', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2648, '2020-08-18 13:13:23.147', 'INFO', '', '软件启动', '', 'PTF'),
	(2649, '2020-08-18 13:13:24.318', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2650, '2020-08-18 13:13:24.341', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2651, '2020-08-18 13:13:24.514', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2652, '2020-08-18 13:13:44.408', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2653, '2020-08-18 13:13:44.417', 'INFO', '', '08-18 13:13:44 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2654, '2020-08-18 13:13:54.055', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(2655, '2020-08-18 13:13:54.062', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(2656, '2020-08-18 13:13:54.093', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(2657, '2020-08-18 13:13:54.102', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(2658, '2020-08-18 13:13:54.119', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(2659, '2020-08-18 13:13:54.131', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(2660, '2020-08-18 13:13:54.160', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(2661, '2020-08-18 13:13:54.166', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(2662, '2020-08-18 13:13:54.899', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2663, '2020-08-18 13:13:54.904', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2664, '2020-08-18 13:13:54.907', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2665, '2020-08-18 13:13:56.807', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2666, '2020-08-18 13:13:56.850', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2667, '2020-08-18 13:13:57.917', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2668, '2020-08-18 13:13:57.924', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2669, '2020-08-18 13:13:57.926', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2670, '2020-08-18 13:14:00.928', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2671, '2020-08-18 13:14:00.931', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2672, '2020-08-18 13:14:00.934', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2673, '2020-08-18 13:14:03.938', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2674, '2020-08-18 13:14:03.941', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2675, '2020-08-18 13:14:03.943', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2676, '2020-08-18 13:14:06.947', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2677, '2020-08-18 13:14:06.950', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2678, '2020-08-18 13:14:06.952', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2679, '2020-08-18 13:14:09.956', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2680, '2020-08-18 13:14:09.959', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2681, '2020-08-18 13:14:09.961', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2682, '2020-08-18 13:14:12.963', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2683, '2020-08-18 13:14:12.967', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2684, '2020-08-18 13:14:12.969', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2685, '2020-08-18 13:14:15.973', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2686, '2020-08-18 13:14:15.975', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2687, '2020-08-18 13:14:15.977', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2688, '2020-08-18 13:14:18.983', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2689, '2020-08-18 13:14:18.987', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2690, '2020-08-18 13:14:18.990', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2691, '2020-08-18 13:14:21.995', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2692, '2020-08-18 13:14:21.998', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2693, '2020-08-18 13:14:22.000', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2694, '2020-08-18 13:14:25.009', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2695, '2020-08-18 13:14:25.013', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2696, '2020-08-18 13:14:25.016', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2697, '2020-08-18 13:14:28.019', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2698, '2020-08-18 13:14:28.022', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2699, '2020-08-18 13:14:28.026', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2700, '2020-08-18 13:14:31.028', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2701, '2020-08-18 13:14:31.032', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2702, '2020-08-18 13:14:31.038', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2703, '2020-08-18 13:14:34.044', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2704, '2020-08-18 13:14:34.053', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2705, '2020-08-18 13:14:34.059', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2706, '2020-08-18 13:14:37.072', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2707, '2020-08-18 13:14:37.082', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2708, '2020-08-18 13:14:37.090', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2709, '2020-08-18 13:14:40.097', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2710, '2020-08-18 13:14:40.101', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2711, '2020-08-18 13:14:40.105', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2712, '2020-08-18 13:14:43.108', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2713, '2020-08-18 13:14:43.112', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2714, '2020-08-18 13:14:43.116', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2715, '2020-08-18 13:14:46.119', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2716, '2020-08-18 13:14:46.123', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2717, '2020-08-18 13:14:46.126', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2718, '2020-08-18 13:14:49.132', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2719, '2020-08-18 13:14:49.136', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2720, '2020-08-18 13:14:49.140', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2721, '2020-08-18 13:14:52.146', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2722, '2020-08-18 13:14:52.149', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2723, '2020-08-18 13:14:52.153', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2724, '2020-08-18 13:14:55.157', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2725, '2020-08-18 13:14:55.160', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2726, '2020-08-18 13:14:55.164', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2727, '2020-08-18 13:14:58.168', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2728, '2020-08-18 13:14:58.173', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2729, '2020-08-18 13:14:58.177', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2730, '2020-08-18 13:15:01.181', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2731, '2020-08-18 13:15:01.186', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2732, '2020-08-18 13:15:01.189', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2733, '2020-08-18 13:15:04.194', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2734, '2020-08-18 13:15:05.199', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2735, '2020-08-18 13:15:05.203', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2736, '2020-08-18 13:15:07.209', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2737, '2020-08-18 13:15:07.213', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2738, '2020-08-18 13:15:07.216', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2739, '2020-08-18 13:15:10.220', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2740, '2020-08-18 13:15:10.225', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2741, '2020-08-18 13:15:10.228', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2742, '2020-08-18 13:15:13.232', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2743, '2020-08-18 13:15:13.240', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2744, '2020-08-18 13:15:13.243', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2745, '2020-08-18 13:15:16.248', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2746, '2020-08-18 13:15:16.252', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2747, '2020-08-18 13:15:16.255', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2748, '2020-08-18 13:15:19.264', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2749, '2020-08-18 13:15:19.270', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2750, '2020-08-18 13:15:19.274', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2751, '2020-08-18 13:15:22.278', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2752, '2020-08-18 13:15:22.281', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2753, '2020-08-18 13:15:22.286', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2754, '2020-08-18 13:15:25.290', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2755, '2020-08-18 13:15:25.305', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2756, '2020-08-18 13:15:25.311', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2757, '2020-08-18 13:15:28.315', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2758, '2020-08-18 13:15:28.323', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2759, '2020-08-18 13:15:28.326', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2760, '2020-08-18 13:15:31.332', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2761, '2020-08-18 13:15:31.336', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2762, '2020-08-18 13:15:31.340', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2763, '2020-08-18 13:15:34.344', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2764, '2020-08-18 13:15:34.352', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2765, '2020-08-18 13:15:34.356', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2766, '2020-08-18 13:15:37.359', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2767, '2020-08-18 13:15:37.363', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2768, '2020-08-18 13:15:37.367', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2769, '2020-08-18 13:15:40.370', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2770, '2020-08-18 13:15:40.374', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2771, '2020-08-18 13:15:40.378', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2772, '2020-08-18 13:15:43.381', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2773, '2020-08-18 13:15:44.383', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2774, '2020-08-18 13:15:44.390', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2775, '2020-08-18 13:15:46.393', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2776, '2020-08-18 13:15:46.396', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2777, '2020-08-18 13:15:46.400', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2778, '2020-08-18 13:15:49.406', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2779, '2020-08-18 13:15:49.412', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2780, '2020-08-18 13:15:49.415', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2781, '2020-08-18 13:15:52.422', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2782, '2020-08-18 13:15:52.425', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2783, '2020-08-18 13:15:52.429', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2784, '2020-08-18 13:15:55.431', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2785, '2020-08-18 13:15:55.435', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2786, '2020-08-18 13:15:55.439', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2787, '2020-08-18 13:15:58.445', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2788, '2020-08-18 13:15:58.449', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2789, '2020-08-18 13:15:58.452', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2790, '2020-08-18 13:16:01.458', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2791, '2020-08-18 13:16:01.463', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2792, '2020-08-18 13:16:01.467', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2793, '2020-08-18 13:16:04.473', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2794, '2020-08-18 13:16:04.476', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2795, '2020-08-18 13:16:04.480', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2796, '2020-08-18 13:16:07.484', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2797, '2020-08-18 13:16:07.489', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2798, '2020-08-18 13:16:07.493', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2799, '2020-08-18 13:16:10.496', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2800, '2020-08-18 13:16:10.500', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2801, '2020-08-18 13:16:10.506', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2802, '2020-08-18 13:16:13.512', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2803, '2020-08-18 13:16:13.516', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2804, '2020-08-18 13:16:13.520', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2805, '2020-08-18 13:16:16.527', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2806, '2020-08-18 13:16:16.531', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2807, '2020-08-18 13:16:16.534', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2808, '2020-08-18 13:16:19.543', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2809, '2020-08-18 13:16:19.547', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2810, '2020-08-18 13:16:19.551', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2811, '2020-08-18 13:16:22.554', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2812, '2020-08-18 13:16:22.562', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2813, '2020-08-18 13:16:22.565', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2814, '2020-08-18 13:16:25.567', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2815, '2020-08-18 13:16:25.571', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2816, '2020-08-18 13:16:25.577', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2817, '2020-08-18 13:16:28.585', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2818, '2020-08-18 13:16:28.592', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2819, '2020-08-18 13:16:28.599', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2820, '2020-08-18 13:16:31.605', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2821, '2020-08-18 13:16:31.609', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2822, '2020-08-18 13:16:31.612', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2823, '2020-08-18 13:16:34.618', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2824, '2020-08-18 13:16:34.622', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2825, '2020-08-18 13:16:34.627', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2826, '2020-08-18 13:16:37.630', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2827, '2020-08-18 13:16:37.645', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2828, '2020-08-18 13:16:37.657', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2829, '2020-08-18 13:16:40.660', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2830, '2020-08-18 13:16:40.665', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2831, '2020-08-18 13:16:40.669', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2832, '2020-08-18 13:16:43.679', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2833, '2020-08-18 13:16:43.685', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2834, '2020-08-18 13:16:43.690', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2835, '2020-08-18 13:16:46.701', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2836, '2020-08-18 13:16:46.705', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2837, '2020-08-18 13:16:46.711', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2838, '2020-08-18 13:16:49.728', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2839, '2020-08-18 13:16:49.741', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2840, '2020-08-18 13:16:49.745', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2841, '2020-08-18 13:16:52.747', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2842, '2020-08-18 13:16:52.751', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2843, '2020-08-18 13:16:52.755', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2844, '2020-08-18 13:16:55.760', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2845, '2020-08-18 13:16:55.764', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2846, '2020-08-18 13:16:55.767', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2847, '2020-08-18 13:16:58.773', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2848, '2020-08-18 13:16:58.778', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2849, '2020-08-18 13:16:58.783', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2850, '2020-08-18 13:17:01.790', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2851, '2020-08-18 13:17:01.794', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2852, '2020-08-18 13:17:01.797', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2853, '2020-08-18 13:17:04.801', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2854, '2020-08-18 13:17:04.810', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2855, '2020-08-18 13:17:04.814', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2856, '2020-08-18 13:17:07.818', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2857, '2020-08-18 13:17:07.832', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2858, '2020-08-18 13:17:07.841', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2859, '2020-08-18 13:17:10.846', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2860, '2020-08-18 13:17:10.851', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2861, '2020-08-18 13:17:10.855', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2862, '2020-08-18 13:17:13.861', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2863, '2020-08-18 13:17:13.865', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2864, '2020-08-18 13:17:13.869', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2865, '2020-08-18 13:17:16.874', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2866, '2020-08-18 13:17:16.878', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2867, '2020-08-18 13:17:16.881', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2868, '2020-08-18 13:17:19.883', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2869, '2020-08-18 13:17:19.887', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2870, '2020-08-18 13:17:19.891', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2871, '2020-08-18 13:17:22.893', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2872, '2020-08-18 13:17:22.897', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2873, '2020-08-18 13:17:22.901', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2874, '2020-08-18 13:17:25.906', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2875, '2020-08-18 13:17:25.911', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2876, '2020-08-18 13:17:25.915', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2877, '2020-08-18 13:17:28.921', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2878, '2020-08-18 13:17:28.926', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2879, '2020-08-18 13:17:28.932', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2880, '2020-08-18 13:17:31.937', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2881, '2020-08-18 13:17:31.941', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2882, '2020-08-18 13:17:31.945', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2883, '2020-08-18 13:17:34.947', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2884, '2020-08-18 13:17:34.951', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2885, '2020-08-18 13:17:34.955', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2886, '2020-08-18 13:17:37.961', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2887, '2020-08-18 13:17:37.965', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2888, '2020-08-18 13:17:37.968', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2889, '2020-08-18 13:17:40.976', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2890, '2020-08-18 13:17:40.980', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2891, '2020-08-18 13:17:40.984', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2892, '2020-08-18 13:17:43.990', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2893, '2020-08-18 13:17:43.994', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2894, '2020-08-18 13:17:43.997', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2895, '2020-08-18 13:17:47.003', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2896, '2020-08-18 13:17:47.009', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2897, '2020-08-18 13:17:47.012', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2898, '2020-08-18 13:17:50.014', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2899, '2020-08-18 13:17:50.018', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2900, '2020-08-18 13:17:50.022', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2901, '2020-08-18 13:17:53.031', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2902, '2020-08-18 13:17:53.036', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2903, '2020-08-18 13:17:53.040', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2904, '2020-08-18 13:17:56.046', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2905, '2020-08-18 13:17:56.050', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2906, '2020-08-18 13:17:56.053', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2907, '2020-08-18 13:17:59.059', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2908, '2020-08-18 13:17:59.062', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2909, '2020-08-18 13:17:59.066', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2910, '2020-08-18 13:18:02.068', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2911, '2020-08-18 13:18:02.074', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2912, '2020-08-18 13:18:02.078', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2913, '2020-08-18 13:18:05.082', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2914, '2020-08-18 13:18:05.091', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2915, '2020-08-18 13:18:05.094', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2916, '2020-08-18 13:18:08.105', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2917, '2020-08-18 13:18:08.108', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2918, '2020-08-18 13:18:08.112', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2919, '2020-08-18 13:18:11.117', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2920, '2020-08-18 13:18:11.121', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2921, '2020-08-18 13:18:11.124', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2922, '2020-08-18 13:18:14.129', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2923, '2020-08-18 13:18:14.133', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2924, '2020-08-18 13:18:14.138', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2925, '2020-08-18 13:18:17.141', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2926, '2020-08-18 13:18:17.144', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2927, '2020-08-18 13:18:17.148', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2928, '2020-08-18 13:18:20.150', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2929, '2020-08-18 13:18:20.154', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2930, '2020-08-18 13:18:20.157', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2931, '2020-08-18 13:18:23.163', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2932, '2020-08-18 13:18:23.167', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2933, '2020-08-18 13:18:23.170', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2934, '2020-08-18 13:18:26.190', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2935, '2020-08-18 13:18:26.200', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2936, '2020-08-18 13:18:26.207', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2937, '2020-08-18 13:18:49.768', 'INFO', '', '软件启动', '', 'PTF'),
	(2938, '2020-08-18 13:18:50.934', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2939, '2020-08-18 13:18:50.993', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2940, '2020-08-18 13:18:51.120', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2941, '2020-08-18 13:18:54.907', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2942, '2020-08-18 13:18:55.013', 'INFO', '', '08-18 13:18:55 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2943, '2020-08-18 13:18:55.054', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(2944, '2020-08-18 13:18:55.060', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(2945, '2020-08-18 13:18:55.087', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(2946, '2020-08-18 13:18:55.103', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(2947, '2020-08-18 13:18:55.134', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(2948, '2020-08-18 13:18:55.145', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(2949, '2020-08-18 13:18:55.577', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2950, '2020-08-18 13:18:55.591', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2951, '2020-08-18 13:18:55.901', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(2952, '2020-08-18 13:18:55.904', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(2953, '2020-08-18 13:19:25.027', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(2954, '2020-08-18 13:24:36.785', 'INFO', '', '软件启动', '', 'PTF'),
	(2955, '2020-08-18 13:24:37.898', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2956, '2020-08-18 13:24:37.964', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2957, '2020-08-18 13:24:38.087', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2958, '2020-08-18 13:24:41.870', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2959, '2020-08-18 13:24:42.003', 'INFO', '', '08-18 13:24:42 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2960, '2020-08-18 13:24:42.020', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(2961, '2020-08-18 13:24:42.032', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(2962, '2020-08-18 13:24:42.061', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(2963, '2020-08-18 13:24:42.083', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(2964, '2020-08-18 13:24:42.113', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(2965, '2020-08-18 13:24:42.119', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(2966, '2020-08-18 13:25:16.640', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2967, '2020-08-18 13:25:16.679', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2968, '2020-08-18 13:30:05.679', 'Error', '', 'MES response timeout', '', 'PTF'),
	(2969, '2020-08-18 13:30:15.728', 'Error', '', 'MES response timeout', '', 'PTF'),
	(2970, '2020-08-18 13:30:25.765', 'Error', '', 'MES response timeout', '', 'PTF'),
	(2971, '2020-08-18 13:30:25.769', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(2972, '2020-08-18 13:30:25.772', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(2973, '2020-08-18 13:30:35.784', 'Error', '', '系统捕获Error：System.Threading.ThreadAbortException: 正在中止线程。\r\n   在 System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne()\r\n   在 NetworkControl.ClientConnectionManager.ExeCmdProc()\r\n   在 System.Threading.ThreadHelper.ThreadStart_Context(Object state)\r\n   在 System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   在 System.Threading.ThreadHelper.ThreadStart()', '', 'PTF'),
	(2974, '2020-08-18 13:44:00.520', 'INFO', '', '软件启动', '', 'PTF'),
	(2975, '2020-08-18 13:44:01.803', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2976, '2020-08-18 13:44:01.864', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2977, '2020-08-18 13:44:05.775', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2978, '2020-08-18 13:44:05.882', 'INFO', '', '08-18 13:44:05 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2979, '2020-08-18 13:44:08.649', 'INFO', '', '系统配置错误', '', 'PTF'),
	(2980, '2020-08-18 13:44:31.355', 'INFO', '', '软件启动', '', 'PTF'),
	(2981, '2020-08-18 13:44:33.551', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2982, '2020-08-18 13:44:33.618', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2983, '2020-08-18 13:44:37.509', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2984, '2020-08-18 13:44:37.644', 'INFO', '', '08-18 13:44:37 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2985, '2020-08-18 13:44:39.505', 'INFO', '', '系统配置错误', '', 'PTF'),
	(2986, '2020-08-18 13:47:19.424', 'INFO', '', '软件启动', '', 'PTF'),
	(2987, '2020-08-18 13:47:20.660', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2988, '2020-08-18 13:47:20.741', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2989, '2020-08-18 13:47:20.877', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2990, '2020-08-18 13:47:22.602', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(2991, '2020-08-18 13:47:24.068', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(2992, '2020-08-18 13:47:24.633', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2993, '2020-08-18 13:47:24.738', 'INFO', '', '08-18 13:47:24 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2994, '2020-08-18 13:47:24.782', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(2995, '2020-08-18 13:47:24.788', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(2996, '2020-08-18 13:47:24.820', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(2997, '2020-08-18 13:47:24.837', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(2998, '2020-08-18 13:47:24.868', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(2999, '2020-08-18 13:47:24.873', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3000, '2020-08-18 13:47:26.139', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3001, '2020-08-18 13:47:26.152', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3002, '2020-08-18 13:49:23.853', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3003, '2020-08-18 13:50:07.439', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3004, '2020-08-18 13:50:20.441', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(3005, '2020-08-18 13:50:20.754', 'Error', '', 'PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error', '', 'PTF'),
	(3006, '2020-08-18 14:06:01.453', 'INFO', '', '软件启动', '', 'PTF'),
	(3007, '2020-08-18 14:06:02.814', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3008, '2020-08-18 14:06:02.882', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3009, '2020-08-18 14:06:03.000', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3010, '2020-08-18 14:07:28.664', 'INFO', '', '软件启动', '', 'PTF'),
	(3011, '2020-08-18 14:07:29.933', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3012, '2020-08-18 14:07:29.993', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3013, '2020-08-18 14:07:30.120', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3014, '2020-08-18 14:07:46.796', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3015, '2020-08-18 14:07:46.932', 'INFO', '', '08-18 14:07:46 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3016, '2020-08-18 14:07:46.953', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3017, '2020-08-18 14:07:46.959', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3018, '2020-08-18 14:07:47.002', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3019, '2020-08-18 14:07:47.019', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3020, '2020-08-18 14:07:47.050', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3021, '2020-08-18 14:07:47.056', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3022, '2020-08-18 14:11:52.459', 'INFO', '', '软件启动', '', 'PTF'),
	(3023, '2020-08-18 14:11:53.664', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3024, '2020-08-18 14:11:53.745', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3025, '2020-08-18 14:11:53.885', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3026, '2020-08-18 14:11:57.619', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3027, '2020-08-18 14:11:57.728', 'INFO', '', '08-18 14:11:57 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3028, '2020-08-18 14:11:57.783', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3029, '2020-08-18 14:11:57.790', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3030, '2020-08-18 14:11:57.819', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3031, '2020-08-18 14:11:57.835', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3032, '2020-08-18 14:11:57.863', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3033, '2020-08-18 14:11:57.874', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3034, '2020-08-18 14:11:59.111', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3035, '2020-08-18 14:11:59.153', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3036, '2020-08-18 14:54:24.349', 'INFO', '', '软件启动', '', 'PTF'),
	(3037, '2020-08-18 14:54:25.599', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3038, '2020-08-18 14:54:25.664', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3039, '2020-08-18 14:54:25.734', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3040, '2020-08-18 14:54:28.096', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(3041, '2020-08-18 14:54:29.611', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3042, '2020-08-18 14:54:29.744', 'INFO', '', '08-18 14:54:29 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3043, '2020-08-18 14:54:29.820', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3044, '2020-08-18 14:54:29.826', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3045, '2020-08-18 14:54:29.856', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3046, '2020-08-18 14:54:29.873', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3047, '2020-08-18 14:54:29.890', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3048, '2020-08-18 14:54:29.896', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3049, '2020-08-18 14:54:29.931', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3050, '2020-08-18 14:54:30.762', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3051, '2020-08-18 14:54:30.776', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3052, '2020-08-18 14:56:08.386', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3053, '2020-08-18 14:59:40.102', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3054, '2020-08-18 15:02:29.584', 'INFO', '', '软件启动', '', 'PTF'),
	(3055, '2020-08-18 15:02:30.843', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3056, '2020-08-18 15:02:30.864', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3057, '2020-08-18 15:02:30.957', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3058, '2020-08-18 15:02:43.123', 'INFO', '', '软件启动', '', 'PTF'),
	(3059, '2020-08-18 15:02:44.375', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3060, '2020-08-18 15:02:44.436', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3061, '2020-08-18 15:02:44.511', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3062, '2020-08-18 15:02:47.964', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(3063, '2020-08-18 15:02:48.336', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3064, '2020-08-18 15:02:48.444', 'INFO', '', '08-18 15:02:48 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3065, '2020-08-18 15:02:48.493', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3066, '2020-08-18 15:02:48.499', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3067, '2020-08-18 15:02:48.524', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3068, '2020-08-18 15:02:48.540', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3069, '2020-08-18 15:02:48.556', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3070, '2020-08-18 15:02:48.562', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3071, '2020-08-18 15:02:48.583', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3072, '2020-08-18 15:02:48.610', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3073, '2020-08-18 15:02:49.256', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3074, '2020-08-18 15:02:49.270', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3075, '2020-08-18 15:11:41.711', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3076, '2020-08-18 15:11:41.720', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(3077, '2020-08-18 15:11:41.713', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(3078, '2020-08-18 15:12:24.847', 'Error', '', '系统捕获Error：System.Threading.ThreadAbortException: 正在中止线程。\r\n   在 System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne()\r\n   在 NetworkControl.ClientConnectionManager.ExeCmdProc()\r\n   在 System.Threading.ThreadHelper.ThreadStart_Context(Object state)\r\n   在 System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   在 System.Threading.ThreadHelper.ThreadStart()', '', 'PTF'),
	(3079, '2020-08-18 15:13:31.532', 'INFO', '', '软件启动', '', 'PTF'),
	(3080, '2020-08-18 15:13:32.654', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3081, '2020-08-18 15:13:32.716', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3082, '2020-08-18 15:13:32.789', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3083, '2020-08-18 15:13:36.620', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3084, '2020-08-18 15:13:36.725', 'INFO', '', '08-18 15:13:36 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3085, '2020-08-18 15:13:37.365', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3086, '2020-08-18 15:13:37.393', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3087, '2020-08-18 15:13:38.061', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3088, '2020-08-18 15:13:38.069', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3089, '2020-08-18 15:13:38.136', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3090, '2020-08-18 15:13:38.143', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3091, '2020-08-18 15:13:38.164', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3092, '2020-08-18 15:13:38.177', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3093, '2020-08-18 15:13:38.214', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3094, '2020-08-18 15:15:26.579', 'INFO', '', '软件启动', '', 'PTF'),
	(3095, '2020-08-18 15:15:27.722', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3096, '2020-08-18 15:15:27.782', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3097, '2020-08-18 15:15:27.848', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3098, '2020-08-18 15:15:31.698', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3099, '2020-08-18 15:15:31.842', 'INFO', '', '08-18 15:15:31 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3100, '2020-08-18 15:15:33.140', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3101, '2020-08-18 15:15:33.145', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3102, '2020-08-18 15:15:33.150', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3103, '2020-08-18 15:15:33.172', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3104, '2020-08-18 15:15:33.222', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3105, '2020-08-18 15:15:33.240', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3106, '2020-08-18 15:15:33.281', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3107, '2020-08-18 15:22:42.930', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3108, '2020-08-18 15:22:42.979', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3109, '2020-08-18 15:22:42.994', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3110, '2020-08-18 15:22:43.014', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3111, '2020-08-18 15:23:33.242', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3112, '2020-08-18 15:23:33.246', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3113, '2020-08-18 15:25:33.220', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3114, '2020-08-18 15:25:33.223', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3115, '2020-08-18 15:25:33.235', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3116, '2020-08-18 15:25:33.247', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3117, '2020-08-18 15:26:44.540', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3118, '2020-08-18 15:27:14.322', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3119, '2020-08-18 15:27:15.730', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(3120, '2020-08-18 15:27:16.581', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(3121, '2020-08-18 15:27:16.582', 'Error', '', 'PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error', '', 'PTF'),
	(3122, '2020-08-18 15:27:27.799', 'Error', '', '系统捕获Error：System.Threading.ThreadAbortException: 正在中止线程。\r\n   在 System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne()\r\n   在 NetworkControl.ClientConnectionManager.ExeCmdProc()\r\n   在 System.Threading.ThreadHelper.ThreadStart_Context(Object state)\r\n   在 System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   在 System.Threading.ThreadHelper.ThreadStart()', '', 'PTF'),
	(3123, '2020-08-18 15:28:05.168', 'INFO', '', '连接备用服务器1号失败', '', 'PTF'),
	(3124, '2020-08-18 15:28:14.031', 'Error', '', 'TCP Socket Error! MES主服务器连接失败', '', 'PTF'),
	(3125, '2020-08-18 15:28:26.335', 'INFO', '', '连接备用服务器2号失败', '', 'PTF'),
	(3126, '2020-08-18 15:28:31.856', 'Error', '', 'TCP Socket Error! MES主服务器连接失败', '', 'PTF'),
	(3127, '2020-08-18 15:31:45.049', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3128, '2020-08-18 15:31:45.050', 'INFO', '', '连接备用服务器4号失败', '', 'PTF'),
	(3129, '2020-08-18 15:31:45.058', 'Error', '', '系统捕获Error：System.Threading.ThreadAbortException: 正在中止线程。\r\n   在 System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne()\r\n   在 NetworkControl.ClientConnectionManager.ExeCmdProc()\r\n   在 System.Threading.ThreadHelper.ThreadStart_Context(Object state)\r\n   在 System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   在 System.Threading.ThreadHelper.ThreadStart()', '', 'PTF'),
	(3130, '2020-08-18 15:31:59.463', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3131, '2020-08-18 15:32:00.211', 'INFO', '', '主服务器重连接成功！', '', 'PTF'),
	(3132, '2020-08-18 15:32:06.740', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3133, '2020-08-18 15:32:07.306', 'INFO', '', '08-18 15:32:06 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3134, '2020-08-18 15:34:39.789', 'INFO', '', '软件启动', '', 'PTF'),
	(3135, '2020-08-18 15:34:41.103', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3136, '2020-08-18 15:34:41.177', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3137, '2020-08-18 15:34:41.262', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3138, '2020-08-18 15:34:45.060', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3139, '2020-08-18 15:34:45.165', 'INFO', '', '08-18 15:34:45 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3140, '2020-08-18 15:34:45.235', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3141, '2020-08-18 15:34:45.241', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3142, '2020-08-18 15:34:45.274', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3143, '2020-08-18 15:34:45.291', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3144, '2020-08-18 15:34:45.308', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3145, '2020-08-18 15:34:45.314', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3146, '2020-08-18 15:34:45.336', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3147, '2020-08-18 15:34:45.362', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3148, '2020-08-18 15:34:46.201', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3149, '2020-08-18 15:34:46.205', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3150, '2020-08-18 15:34:46.273', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3151, '2020-08-18 15:34:46.285', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3152, '2020-08-18 15:42:45.983', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3153, '2020-08-18 15:51:29.147', 'INFO', '', '软件启动', '', 'PTF'),
	(3154, '2020-08-18 15:51:30.538', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3155, '2020-08-18 15:51:30.600', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3156, '2020-08-18 15:51:30.668', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3157, '2020-08-18 15:51:34.520', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3158, '2020-08-18 15:51:34.644', 'INFO', '', '08-18 15:51:34 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3159, '2020-08-18 15:52:09.785', 'Error', '', 'MES response timeout', '', 'PTF'),
	(3160, '2020-08-18 15:53:45.798', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(3161, '2020-08-18 15:53:49.758', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(3162, '2020-08-18 15:53:57.783', 'Error', '', '系统捕获Error：System.Threading.ThreadAbortException: 正在中止线程。\r\n   在 System.Threading.WaitHandle.WaitOneNative(SafeHandle waitableSafeHandle, UInt32 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext)\r\n   在 System.Threading.WaitHandle.WaitOne()\r\n   在 NetworkControl.ClientConnectionManager.ExeCmdProc()\r\n   在 System.Threading.ThreadHelper.ThreadStart_Context(Object state)\r\n   在 System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)\r\n   在 System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)\r\n   在 System.Threading.ThreadHelper.ThreadStart()', '', 'PTF'),
	(3163, '2020-08-18 15:54:35.659', 'INFO', '', '连接备用服务器1号失败', '', 'PTF'),
	(3164, '2020-08-18 15:56:08.821', 'INFO', '', '软件启动', '', 'PTF'),
	(3165, '2020-08-18 15:56:10.154', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3166, '2020-08-18 15:56:10.202', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3167, '2020-08-18 15:56:10.296', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3168, '2020-08-18 15:56:14.127', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3169, '2020-08-18 15:56:14.251', 'INFO', '', '08-18 15:56:14 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3170, '2020-08-18 16:05:32.043', 'INFO', '', '软件启动', '', 'PTF'),
	(3171, '2020-08-18 16:05:33.239', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3172, '2020-08-18 16:05:33.288', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3173, '2020-08-18 16:05:33.402', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3174, '2020-08-18 16:05:37.189', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3175, '2020-08-18 16:05:37.319', 'INFO', '', '08-18 16:05:37 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3176, '2020-08-18 16:05:45.359', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3177, '2020-08-18 16:05:45.368', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3178, '2020-08-18 16:05:45.401', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3179, '2020-08-18 16:05:45.418', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3180, '2020-08-18 16:05:45.435', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3181, '2020-08-18 16:05:45.441', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3182, '2020-08-18 16:05:45.464', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3183, '2020-08-18 16:05:45.490', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3184, '2020-08-18 16:05:47.909', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3185, '2020-08-18 16:05:47.924', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3186, '2020-08-18 16:09:08.894', 'INFO', '', '软件启动', '', 'PTF'),
	(3187, '2020-08-18 16:09:10.310', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3188, '2020-08-18 16:09:10.402', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3189, '2020-08-18 16:09:10.489', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3190, '2020-08-18 16:09:14.288', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3191, '2020-08-18 16:09:14.421', 'INFO', '', '08-18 16:09:14 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3192, '2020-08-18 16:09:14.575', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3193, '2020-08-18 16:09:14.541', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3194, '2020-08-18 16:09:14.600', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3195, '2020-08-18 16:09:14.616', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3196, '2020-08-18 16:09:14.632', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3197, '2020-08-18 16:09:14.638', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3198, '2020-08-18 16:09:14.659', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3199, '2020-08-18 16:09:14.687', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3200, '2020-08-18 16:09:15.667', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3201, '2020-08-18 16:09:15.680', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3202, '2020-08-18 16:09:23.981', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3203, '2020-08-18 16:09:24.012', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3204, '2020-08-18 16:15:13.073', 'INFO', '', '软件启动', '', 'PTF'),
	(3205, '2020-08-18 16:15:14.418', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3206, '2020-08-18 16:15:14.471', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3207, '2020-08-18 16:15:14.576', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3208, '2020-08-18 16:15:30.971', 'INFO', '', '软件启动', '', 'PTF'),
	(3209, '2020-08-18 16:15:32.367', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3210, '2020-08-18 16:15:32.501', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3211, '2020-08-18 16:15:33.371', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3212, '2020-08-18 16:15:37.312', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3213, '2020-08-18 16:15:37.418', 'INFO', '', '08-18 16:15:37 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3214, '2020-08-18 16:15:37.520', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3215, '2020-08-18 16:15:37.526', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3216, '2020-08-18 16:15:37.588', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3217, '2020-08-18 16:15:37.604', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3218, '2020-08-18 16:15:37.651', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3219, '2020-08-18 16:15:38.903', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3220, '2020-08-18 16:15:38.943', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3221, '2020-08-18 16:17:15.963', 'INFO', '', '软件启动', '', 'PTF'),
	(3222, '2020-08-18 16:17:17.254', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3223, '2020-08-18 16:17:17.343', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3224, '2020-08-18 16:17:17.435', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3225, '2020-08-18 16:17:17.652', 'Error', '', '读取地址段ID：3，地址段名称input监控地址错误：Ads-Error 0x2751 : A unknown Ads-Error has occurred.\r\n   在 TwinCAT.Ads.Internal.AdsRawInterceptor.ThrowAdsException(AdsErrorCode adsErrorCode)\r\n   在 TwinCAT.Ads.Internal.AdsRawInterceptor.ReadAny(UInt32 indexGroup, UInt32 indexOffset, Type type, Int32[] args, Boolean throwAdsException, Object& value)\r\n   在 TwinCAT.Ads.TcAdsClient.ReadAny(Int64 indexGroup, Int64 indexOffset, Type type, Int32[] args)\r\n   在 ATL.Engine.AdsPlc_D.Read[T](String address, String& Msg, UInt16 length) 位置 E:MTW20200807ATL.EngineAdsPlc_D.cs:行号 143', '', 'PTF'),
	(3226, '2020-08-18 16:17:24.245', 'Error', '', '读取地址段ID：3，地址段名称input监控地址错误：Ads-Error 0x2751 : A unknown Ads-Error has occurred.\r\n   在 TwinCAT.Ads.Internal.AdsRawInterceptor.ThrowAdsException(AdsErrorCode adsErrorCode)\r\n   在 TwinCAT.Ads.Internal.AdsRawInterceptor.ReadAny(UInt32 indexGroup, UInt32 indexOffset, Type type, Int32[] args, Boolean throwAdsException, Object& value)\r\n   在 TwinCAT.Ads.TcAdsClient.ReadAny(Int64 indexGroup, Int64 indexOffset, Type type, Int32[] args)\r\n   在 ATL.Engine.AdsPlc_D.Read[T](String address, String& Msg, UInt16 length) 位置 E:MTW20200807ATL.EngineAdsPlc_D.cs:行号 143', '', 'PTF'),
	(3227, '2020-08-18 16:17:55.320', 'INFO', '', '软件启动', '', 'PTF'),
	(3228, '2020-08-18 16:17:56.604', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3229, '2020-08-18 16:17:56.659', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3230, '2020-08-18 16:17:56.772', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3231, '2020-08-18 16:24:09.322', 'INFO', '', '软件启动', '', 'PTF'),
	(3232, '2020-08-18 16:24:10.639', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3233, '2020-08-18 16:24:10.695', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3234, '2020-08-18 16:24:14.607', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3235, '2020-08-18 16:24:14.740', 'INFO', '', '08-18 16:24:14 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3236, '2020-08-18 16:24:21.119', 'INFO', '', '系统配置错误', '', 'PTF'),
	(3237, '2020-08-18 16:24:39.095', 'INFO', '', '软件启动', '', 'PTF'),
	(3238, '2020-08-18 16:24:40.533', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3239, '2020-08-18 16:24:40.612', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3240, '2020-08-18 16:24:44.479', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3241, '2020-08-18 16:24:44.615', 'INFO', '', '08-18 16:24:44 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3242, '2020-08-18 16:24:50.495', 'INFO', '', '系统配置错误', '', 'PTF'),
	(3243, '2020-08-18 16:26:37.307', 'INFO', '', '软件启动', '', 'PTF'),
	(3244, '2020-08-18 16:26:38.644', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3245, '2020-08-18 16:26:38.736', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3246, '2020-08-18 16:26:42.618', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3247, '2020-08-18 16:26:42.625', 'INFO', '', '系统配置错误', '', 'PTF'),
	(3248, '2020-08-18 16:26:42.754', 'INFO', '', '08-18 16:26:42 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3249, '2020-08-18 16:29:01.636', 'INFO', '', '软件启动', '', 'PTF'),
	(3250, '2020-08-18 16:29:03.067', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3251, '2020-08-18 16:29:04.096', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3252, '2020-08-18 16:29:08.042', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3253, '2020-08-18 16:29:08.150', 'INFO', '', '08-18 16:29:08 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3254, '2020-08-18 16:29:09.405', 'INFO', '', '系统配置错误', '', 'PTF'),
	(3255, '2020-08-18 16:30:51.533', 'INFO', '', '软件启动', '', 'PTF'),
	(3256, '2020-08-18 16:30:52.990', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3257, '2020-08-18 16:30:53.105', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3258, '2020-08-18 16:31:19.817', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3259, '2020-08-18 16:47:29.767', 'INFO', '', '软件启动', '', 'PTF'),
	(3260, '2020-08-18 16:47:31.169', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3261, '2020-08-18 16:47:31.240', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3262, '2020-08-18 16:47:31.331', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3263, '2020-08-18 16:48:11.958', 'INFO', '', '软件启动', '', 'PTF'),
	(3264, '2020-08-18 16:48:13.404', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3265, '2020-08-18 16:48:13.463', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3266, '2020-08-18 16:48:13.570', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3267, '2020-08-18 16:48:16.968', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(3268, '2020-08-18 16:48:17.379', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3269, '2020-08-18 16:48:17.510', 'INFO', '', '08-18 16:48:17 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3270, '2020-08-18 16:48:17.535', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3271, '2020-08-18 16:48:17.543', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3272, '2020-08-18 16:48:17.572', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3273, '2020-08-18 16:48:17.589', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3274, '2020-08-18 16:48:17.609', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3275, '2020-08-18 16:48:17.615', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3276, '2020-08-18 16:48:17.637', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3277, '2020-08-18 16:48:17.664', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3278, '2020-08-18 16:48:18.504', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3279, '2020-08-18 16:48:18.507', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3280, '2020-08-18 16:48:19.342', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3281, '2020-08-18 16:48:19.355', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3282, '2020-08-18 16:49:42.591', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3283, '2020-08-18 16:49:42.593', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3284, '2020-08-18 16:51:55.107', 'INFO', '', '软件启动', '', 'PTF'),
	(3285, '2020-08-18 16:51:56.522', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3286, '2020-08-18 16:51:56.582', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3287, '2020-08-18 16:51:57.461', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3288, '2020-08-18 16:52:00.462', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3289, '2020-08-18 16:52:00.580', 'INFO', '', '08-18 16:52:00 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3290, '2020-08-18 16:52:01.900', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3291, '2020-08-18 16:52:01.918', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3292, '2020-08-18 16:54:16.356', 'INFO', '', '软件启动', '', 'PTF'),
	(3293, '2020-08-18 16:54:17.934', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3294, '2020-08-18 16:54:17.977', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3295, '2020-08-18 16:54:23.184', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3296, '2020-08-18 16:54:26.154', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3297, '2020-08-18 16:54:26.289', 'INFO', '', '08-18 16:54:26 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3298, '2020-08-18 16:54:27.620', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3299, '2020-08-18 16:54:27.631', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3300, '2020-08-18 16:56:25.066', 'INFO', '', '软件启动', '', 'PTF'),
	(3301, '2020-08-18 16:56:26.423', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3302, '2020-08-18 16:56:26.506', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3303, '2020-08-18 16:56:26.599', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3304, '2020-08-18 16:56:33.941', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3305, '2020-08-18 16:56:34.077', 'INFO', '', '08-18 16:56:34 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3306, '2020-08-18 16:56:34.160', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3307, '2020-08-18 16:56:34.167', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3308, '2020-08-18 16:56:34.196', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3309, '2020-08-18 16:56:34.203', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3310, '2020-08-18 16:56:34.220', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3311, '2020-08-18 16:56:34.235', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3312, '2020-08-18 16:56:34.252', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3313, '2020-08-18 16:56:34.259', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3314, '2020-08-18 16:56:34.281', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3315, '2020-08-18 16:56:34.306', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3316, '2020-08-18 16:56:34.442', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3317, '2020-08-18 16:56:34.454', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3318, '2020-08-18 16:59:31.178', 'INFO', '', '软件启动', '', 'PTF'),
	(3319, '2020-08-18 16:59:32.598', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3320, '2020-08-18 16:59:32.660', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3321, '2020-08-18 16:59:32.763', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3322, '2020-08-18 17:01:14.462', 'INFO', '', '软件启动', '', 'PTF'),
	(3323, '2020-08-18 17:01:15.847', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(3324, '2020-08-18 17:01:15.870', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(3325, '2020-08-18 17:01:15.979', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(3326, '2020-08-18 17:01:17.563', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(3327, '2020-08-18 17:01:19.799', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(3328, '2020-08-18 17:01:19.906', 'INFO', '', '08-18 17:01:19 设备ID:ACOA0022注册成功', '', 'PTF'),
	(3329, '2020-08-18 17:01:19.978', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3330, '2020-08-18 17:01:19.984', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3331, '2020-08-18 17:01:20.012', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3332, '2020-08-18 17:01:20.028', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3333, '2020-08-18 17:01:20.047', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(3334, '2020-08-18 17:01:20.053', 'INFO', '', 'A021发送当前易损件信息给MES ACOA0022', '', 'PTF'),
	(3335, '2020-08-18 17:01:20.075', 'INFO', '', 'MES返回的errmsg：A026---- 00   ', '', 'PTF'),
	(3336, '2020-08-18 17:01:20.103', 'INFO', '', 'MES返回的errmsg：A046---- 00   ', '', 'PTF'),
	(3337, '2020-08-18 17:01:20.435', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(3338, '2020-08-18 17:01:20.422', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(3339, '2020-08-18 17:01:31.864', 'Error', '', '写入MESconnected失败', '', 'PTF'),
	(3340, '2020-08-18 17:01:31.874', 'Error', '', '写入MesReply失败', '', 'PTF'),
	(3341, '2020-08-18 17:01:31.886', 'Error', '', '写入上位机心跳失败', '', 'PTF'),
	(3342, '2020-08-18 17:01:32.086', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3343, '2020-08-18 17:01:32.118', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3344, '2020-08-18 17:01:32.881', 'Error', '', 'PLC连接失败', '', 'PTF'),
	(3345, '2020-08-18 17:02:19.667', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3346, '2020-08-18 17:02:19.699', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(3347, '2020-08-18 17:11:20.009', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(3348, '2020-08-18 17:11:20.041', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(3349, '2020-08-18 17:11:20.047', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(3350, '2020-08-18 17:11:20.062', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF');
/*!40000 ALTER TABLE `log4net` ENABLE KEYS */;

-- Dumping structure for table ptf.loglevel
CREATE TABLE IF NOT EXISTS `loglevel` (
  `loglevel` varchar(50) NOT NULL,
  PRIMARY KEY (`loglevel`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='日志等级表';

-- Dumping data for table ptf.loglevel: ~5 rows (大约)
/*!40000 ALTER TABLE `loglevel` DISABLE KEYS */;
INSERT INTO `loglevel` (`loglevel`) VALUES
	('DEBUG'),
	('ERROR'),
	('FATAL'),
	('INFO'),
	('WARN');
/*!40000 ALTER TABLE `loglevel` ENABLE KEYS */;

-- Dumping structure for table ptf.log_operation
CREATE TABLE IF NOT EXISTS `log_operation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `datatime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `username` varchar(50) NOT NULL COMMENT '当前登录用户',
  `Action` varchar(200) NOT NULL COMMENT '操作内容 /MES ID变更内容',
  PRIMARY KEY (`id`),
  KEY `datatime` (`datatime`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8 COMMENT='操作记录信息/参数变更记录信息\r\n比如操作了某个运行按钮，操作了停止按钮，需要记录在此表\r\n比如软件记录MES ID的值发生了变化，则也需要将MES ID变化前的值和变化后的值记录在此表';

-- Dumping data for table ptf.log_operation: ~11 rows (大约)
/*!40000 ALTER TABLE `log_operation` DISABLE KEYS */;
INSERT INTO `log_operation` (`id`, `datatime`, `username`, `Action`) VALUES
	(15, '2020-08-17 22:00:51.808', 'SuperAdmin', '本地登陆'),
	(16, '2020-08-17 22:16:19.611', 'superAdmin', 'MES登陆'),
	(17, '2020-08-18 11:38:34.842', 'superAdmin', 'MES登陆'),
	(18, '2020-08-18 13:09:35.718', 'superAdmin', 'MES登陆'),
	(19, '2020-08-18 13:10:02.666', 'superAdmin', 'MES登陆'),
	(20, '2020-08-18 13:10:51.635', 'superAdmin', 'MES登陆'),
	(21, '2020-08-18 13:13:56.851', 'superAdmin', 'MES登陆'),
	(22, '2020-08-18 13:18:55.592', 'superAdmin', 'MES登陆'),
	(23, '2020-08-18 13:25:16.680', 'superAdmin', 'MES登陆'),
	(24, '2020-08-18 13:47:26.154', 'superAdmin', 'MES登陆'),
	(25, '2020-08-18 14:11:59.154', 'superAdmin', 'MES登陆'),
	(26, '2020-08-18 14:54:30.778', 'superAdmin', 'MES登陆'),
	(27, '2020-08-18 15:02:49.271', 'superAdmin', 'MES登陆'),
	(28, '2020-08-18 15:13:37.394', 'superAdmin', 'MES登陆'),
	(29, '2020-08-18 15:15:33.173', 'superAdmin', 'MES登陆'),
	(30, '2020-08-18 15:34:46.287', 'superAdmin', 'MES登陆'),
	(31, '2020-08-18 16:05:47.925', 'superAdmin', 'MES登陆'),
	(32, '2020-08-18 16:09:15.681', 'superAdmin', 'MES登陆'),
	(33, '2020-08-18 16:15:38.944', 'superAdmin', 'MES登陆'),
	(34, '2020-08-18 16:48:19.356', 'superAdmin', 'MES登陆'),
	(35, '2020-08-18 16:52:01.920', 'superAdmin', 'MES登陆'),
	(36, '2020-08-18 16:54:27.633', 'superAdmin', 'MES登陆'),
	(37, '2020-08-18 16:56:34.456', 'superAdmin', 'MES登陆'),
	(38, '2020-08-18 17:01:20.436', 'superAdmin', 'MES登陆');
/*!40000 ALTER TABLE `log_operation` ENABLE KEYS */;

-- Dumping structure for table ptf.log_plc_interactive
CREATE TABLE IF NOT EXISTS `log_plc_interactive` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `datatime` datetime(3) NOT NULL COMMENT '记录时间',
  `variable_ID` int(11) NOT NULL COMMENT '变量ID',
  `value` varchar(200) NOT NULL DEFAULT '' COMMENT '读写值',
  `remark` varchar(100) NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `datatime` (`datatime`),
  KEY `FK_log_plc_interactive_user_fefine_variable` (`variable_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=1419 DEFAULT CHARSET=utf8 COMMENT='变量自动监控记录表';

-- Dumping data for table ptf.log_plc_interactive: ~27 rows (大约)
/*!40000 ALTER TABLE `log_plc_interactive` DISABLE KEYS */;
INSERT INTO `log_plc_interactive` (`id`, `datatime`, `variable_ID`, `value`, `remark`) VALUES
	(1337, '2020-08-18 13:13:54.070', 49, '0', ''),
	(1338, '2020-08-18 13:13:54.076', 50, '0', ''),
	(1339, '2020-08-18 13:13:54.084', 51, '0', ''),
	(1340, '2020-08-18 13:18:55.067', 49, '0', ''),
	(1341, '2020-08-18 13:18:55.072', 50, '0', ''),
	(1342, '2020-08-18 13:18:55.079', 51, '0', ''),
	(1343, '2020-08-18 13:24:42.041', 49, '0', ''),
	(1344, '2020-08-18 13:24:42.046', 50, '0', ''),
	(1345, '2020-08-18 13:24:42.053', 51, '0', ''),
	(1346, '2020-08-18 13:24:45.720', 47, '10', ''),
	(1347, '2020-08-18 13:24:49.424', 48, '10', ''),
	(1348, '2020-08-18 13:30:26.911', 47, '20', ''),
	(1349, '2020-08-18 13:47:24.796', 49, '0', ''),
	(1350, '2020-08-18 13:47:24.801', 50, '0', ''),
	(1351, '2020-08-18 13:47:24.809', 51, '0', ''),
	(1352, '2020-08-18 13:47:25.648', 47, '10', ''),
	(1353, '2020-08-18 13:47:25.667', 48, '10', ''),
	(1354, '2020-08-18 14:07:46.973', 49, '0', ''),
	(1355, '2020-08-18 14:07:46.976', 50, '0', ''),
	(1356, '2020-08-18 14:07:46.993', 51, '0', ''),
	(1357, '2020-08-18 14:07:50.300', 47, '10', ''),
	(1358, '2020-08-18 14:08:27.635', 48, '10', ''),
	(1359, '2020-08-18 14:11:57.798', 49, '0', ''),
	(1360, '2020-08-18 14:11:57.803', 50, '0', ''),
	(1361, '2020-08-18 14:11:57.810', 51, '0', ''),
	(1362, '2020-08-18 14:11:58.608', 47, '10', ''),
	(1363, '2020-08-18 14:11:58.613', 48, '99', ''),
	(1364, '2020-08-18 14:54:29.835', 49, '0', ''),
	(1365, '2020-08-18 14:54:29.840', 50, '0', ''),
	(1366, '2020-08-18 14:54:29.847', 51, '0', ''),
	(1367, '2020-08-18 14:54:30.557', 47, '10', ''),
	(1368, '2020-08-18 14:54:30.562', 48, '99', ''),
	(1369, '2020-08-18 15:02:48.506', 49, '0', ''),
	(1370, '2020-08-18 15:02:48.510', 50, '0', ''),
	(1371, '2020-08-18 15:02:48.516', 51, '0', ''),
	(1372, '2020-08-18 15:02:49.336', 47, '10', ''),
	(1373, '2020-08-18 15:02:49.341', 48, '99', ''),
	(1374, '2020-08-18 15:13:38.636', 47, '10', ''),
	(1375, '2020-08-18 15:13:38.651', 48, '99', ''),
	(1376, '2020-08-18 15:15:33.711', 47, '10', ''),
	(1377, '2020-08-18 15:15:33.728', 48, '99', ''),
	(1378, '2020-08-18 15:22:40.702', 49, '0', ''),
	(1379, '2020-08-18 15:22:42.926', 50, '0', ''),
	(1380, '2020-08-18 15:22:42.928', 51, '0', ''),
	(1381, '2020-08-18 15:27:20.070', 47, '20', ''),
	(1382, '2020-08-18 15:32:07.838', 47, '10', ''),
	(1383, '2020-08-18 15:34:45.250', 49, '0', ''),
	(1384, '2020-08-18 15:34:45.257', 50, '0', ''),
	(1385, '2020-08-18 15:34:45.266', 51, '0', ''),
	(1386, '2020-08-18 15:34:46.056', 47, '10', ''),
	(1387, '2020-08-18 15:34:46.060', 48, '99', ''),
	(1388, '2020-08-18 15:51:57.205', 47, '10', ''),
	(1389, '2020-08-18 15:51:57.452', 48, '99', ''),
	(1390, '2020-08-18 15:53:45.800', 47, '20', ''),
	(1391, '2020-08-18 15:56:18.722', 47, '10', ''),
	(1392, '2020-08-18 16:05:45.357', 47, '10', ''),
	(1393, '2020-08-18 16:05:45.363', 48, '99', ''),
	(1394, '2020-08-18 16:05:45.373', 49, '0', ''),
	(1395, '2020-08-18 16:05:45.376', 50, '0', ''),
	(1396, '2020-08-18 16:05:45.393', 51, '0', ''),
	(1397, '2020-08-18 16:09:14.580', 49, '0', ''),
	(1398, '2020-08-18 16:09:14.585', 50, '0', ''),
	(1399, '2020-08-18 16:09:14.592', 51, '0', ''),
	(1400, '2020-08-18 16:09:15.271', 47, '10', ''),
	(1401, '2020-08-18 16:09:15.277', 48, '99', ''),
	(1402, '2020-08-18 16:15:38.354', 47, '10', ''),
	(1403, '2020-08-18 16:15:38.382', 48, '99', ''),
	(1404, '2020-08-18 16:48:17.547', 49, '0', ''),
	(1405, '2020-08-18 16:48:17.555', 50, '0', ''),
	(1406, '2020-08-18 16:48:17.564', 51, '0', ''),
	(1407, '2020-08-18 16:48:18.354', 47, '10', ''),
	(1408, '2020-08-18 16:48:18.359', 48, '99', ''),
	(1409, '2020-08-18 16:56:34.172', 49, '0', ''),
	(1410, '2020-08-18 16:56:34.179', 50, '0', ''),
	(1411, '2020-08-18 16:56:34.188', 51, '0', ''),
	(1412, '2020-08-18 16:56:34.941', 47, '10', ''),
	(1413, '2020-08-18 16:56:34.946', 48, '99', ''),
	(1414, '2020-08-18 17:01:19.988', 49, '0', ''),
	(1415, '2020-08-18 17:01:19.994', 50, '0', ''),
	(1416, '2020-08-18 17:01:20.002', 51, '0', ''),
	(1417, '2020-08-18 17:01:20.817', 47, '10', ''),
	(1418, '2020-08-18 17:01:20.823', 48, '99', '');
/*!40000 ALTER TABLE `log_plc_interactive` ENABLE KEYS */;

-- Dumping structure for table ptf.log_simple_mes_interface_execution
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
  KEY `FunctionID` (`FunctionID`),
  KEY `datatime` (`datatime`)
) ENGINE=InnoDB AUTO_INCREMENT=5212 DEFAULT CHARSET=utf8 COMMENT='MES接口上传数据表，包括了上传MES的信息和MES返回的信息\r\nResoponseTime:上位机上传数据给MES后，MES返回信息的时间，单位为ms';

-- Dumping data for table ptf.log_simple_mes_interface_execution: ~94 rows (大约)
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` DISABLE KEYS */;
INSERT INTO `log_simple_mes_interface_execution` (`id`, `datatime`, `FunctionID`, `GUID`, `ResponseTime`, `RequestTime`, `Data`, `errorMsg`) VALUES
	(4920, '2020-08-17 21:57:32.108', 'A001', '6887fcad-caee-410c-964c-10d303d89e0e', NULL, '2020-08-17 21:57:32.102', '{"Header":{"SessionID":"6887fcad-caee-410c-964c-10d303d89e0e","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.vshost.exe","RequestTime":"2020-08-17 21:57:32 102"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4921, '2020-08-17 21:57:32.156', 'A002', '6887fcad-caee-410c-964c-10d303d89e0e', '2020-08-17 21:57:32.102', '2020-08-17 21:57:32.102', '{"Header":{"SessionID":"6887fcad-caee-410c-964c-10d303d89e0e","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-17 21:57:32 102","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-17 21:57:32 102"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4922, '2020-08-17 22:01:35.726', 'A001', '7c5660d3-be00-447c-85b5-51a716793986', NULL, '2020-08-17 22:01:35.708', '{"Header":{"SessionID":"7c5660d3-be00-447c-85b5-51a716793986","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.vshost.exe","RequestTime":"2020-08-17 22:01:35 708"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4923, '2020-08-17 22:01:35.731', 'A002', '7c5660d3-be00-447c-85b5-51a716793986', '2020-08-17 22:01:35.729', '2020-08-17 22:01:35.729', '{"Header":{"SessionID":"7c5660d3-be00-447c-85b5-51a716793986","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-17 22:01:35 708","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-17 22:01:35 725"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4924, '2020-08-17 22:16:18.668', 'A001', '800e661e-1b89-4754-a0d3-231ee4bfa5e9', NULL, '2020-08-17 22:16:18.662', '{"Header":{"SessionID":"800e661e-1b89-4754-a0d3-231ee4bfa5e9","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.vshost.exe","RequestTime":"2020-08-17 22:16:18 662"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4925, '2020-08-17 22:16:18.673', 'A002', '800e661e-1b89-4754-a0d3-231ee4bfa5e9', '2020-08-17 22:16:18.662', '2020-08-17 22:16:18.662', '{"Header":{"SessionID":"800e661e-1b89-4754-a0d3-231ee4bfa5e9","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-17 22:16:18 662","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-17 22:16:18 662"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4926, '2020-08-17 22:16:19.593', 'A039', '1ce1225c-221b-4b98-8b3e-a44c300069cb', NULL, '2020-08-17 22:16:19.584', '{"Header":{"SessionID":"1ce1225c-221b-4b98-8b3e-a44c300069cb","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.vshost.exe","RequestTime":"2020-08-17 22:16:19 584","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4927, '2020-08-17 22:16:19.596', 'A040', '1ce1225c-221b-4b98-8b3e-a44c300069cb', '2020-08-17 22:16:19.584', '2020-08-17 22:16:19.584', '{"Header":{"SessionID":"1ce1225c-221b-4b98-8b3e-a44c300069cb","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-17 22:16:19 584","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-17 22:16:19 584"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4928, '2020-08-18 11:33:26.083', 'A001', '10737534-584a-49b9-ba72-c00bc5c7678c', NULL, '2020-08-18 11:33:26.078', '{"Header":{"SessionID":"10737534-584a-49b9-ba72-c00bc5c7678c","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 11:33:26 078"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4929, '2020-08-18 11:33:26.126', 'A002', '10737534-584a-49b9-ba72-c00bc5c7678c', '2020-08-18 11:33:26.084', '2020-08-18 11:33:26.084', '{"Header":{"SessionID":"10737534-584a-49b9-ba72-c00bc5c7678c","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 11:33:26 078","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 11:33:26 081"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4930, '2020-08-18 11:36:37.958', 'A001', 'd27eeadf-9ece-4678-93f9-907021a9c68e', NULL, '2020-08-18 11:36:37.951', '{"Header":{"SessionID":"d27eeadf-9ece-4678-93f9-907021a9c68e","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 11:36:37 951"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4931, '2020-08-18 11:36:38.002', 'A002', 'd27eeadf-9ece-4678-93f9-907021a9c68e', '2020-08-18 11:36:37.960', '2020-08-18 11:36:37.960', '{"Header":{"SessionID":"d27eeadf-9ece-4678-93f9-907021a9c68e","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 11:36:37 951","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 11:36:37 956"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4932, '2020-08-18 11:38:29.603', 'A001', 'f859fdca-65ab-4ebb-a9fb-cbac5ae4fe02', NULL, '2020-08-18 11:38:29.598', '{"Header":{"SessionID":"f859fdca-65ab-4ebb-a9fb-cbac5ae4fe02","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 11:38:29 598"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4933, '2020-08-18 11:38:29.650', 'A002', 'f859fdca-65ab-4ebb-a9fb-cbac5ae4fe02', '2020-08-18 11:38:29.607', '2020-08-18 11:38:29.607', '{"Header":{"SessionID":"f859fdca-65ab-4ebb-a9fb-cbac5ae4fe02","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 11:38:29 598","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 11:38:29 602"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4934, '2020-08-18 11:38:34.821', 'A039', '408260cb-e8a0-458d-8d5d-a153f0db08a9', NULL, '2020-08-18 11:38:34.816', '{"Header":{"SessionID":"408260cb-e8a0-458d-8d5d-a153f0db08a9","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 11:38:34 816","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4935, '2020-08-18 11:38:34.824', 'A040', '408260cb-e8a0-458d-8d5d-a153f0db08a9', '2020-08-18 11:38:34.823', '2020-08-18 11:38:34.823', '{"Header":{"SessionID":"408260cb-e8a0-458d-8d5d-a153f0db08a9","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 11:38:34 816","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 11:38:34 819"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4936, '2020-08-18 11:39:01.117', 'A001', '43f76a85-7fcd-4951-84b2-ac47b1fe5d1e', NULL, '2020-08-18 11:39:01.112', '{"Header":{"SessionID":"43f76a85-7fcd-4951-84b2-ac47b1fe5d1e","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 11:39:01 112"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4937, '2020-08-18 11:39:01.166', 'A002', '43f76a85-7fcd-4951-84b2-ac47b1fe5d1e', '2020-08-18 11:39:01.120', '2020-08-18 11:39:01.120', '{"Header":{"SessionID":"43f76a85-7fcd-4951-84b2-ac47b1fe5d1e","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 11:39:01 112","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 11:39:01 115"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4938, '2020-08-18 13:09:34.698', 'A001', 'c3f5da0a-1ee6-43d8-b369-6c531007218d', NULL, '2020-08-18 13:09:34.691', '{"Header":{"SessionID":"c3f5da0a-1ee6-43d8-b369-6c531007218d","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:09:34 691"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4939, '2020-08-18 13:09:34.744', 'A002', 'c3f5da0a-1ee6-43d8-b369-6c531007218d', '2020-08-18 13:09:34.700', '2020-08-18 13:09:34.700', '{"Header":{"SessionID":"c3f5da0a-1ee6-43d8-b369-6c531007218d","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:09:34 691","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:09:34 694"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4940, '2020-08-18 13:09:35.674', 'A039', 'b2552adb-268f-4258-a64a-850db428fcb3', NULL, '2020-08-18 13:09:35.670', '{"Header":{"SessionID":"b2552adb-268f-4258-a64a-850db428fcb3","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:09:35 670","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4941, '2020-08-18 13:09:35.678', 'A040', 'b2552adb-268f-4258-a64a-850db428fcb3', '2020-08-18 13:09:35.677', '2020-08-18 13:09:35.677', '{"Header":{"SessionID":"b2552adb-268f-4258-a64a-850db428fcb3","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:09:35 670","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:09:35 673"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4942, '2020-08-18 13:10:02.138', 'A001', '2851c67a-ad3c-462b-b916-4476f6979588', NULL, '2020-08-18 13:10:02.133', '{"Header":{"SessionID":"2851c67a-ad3c-462b-b916-4476f6979588","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:10:02 133"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4943, '2020-08-18 13:10:02.181', 'A002', '2851c67a-ad3c-462b-b916-4476f6979588', '2020-08-18 13:10:02.140', '2020-08-18 13:10:02.140', '{"Header":{"SessionID":"2851c67a-ad3c-462b-b916-4476f6979588","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:10:02 133","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:10:02 136"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4944, '2020-08-18 13:10:02.620', 'A039', '7e170d3a-f26f-41f4-b2b0-629a5df86baf', NULL, '2020-08-18 13:10:02.616', '{"Header":{"SessionID":"7e170d3a-f26f-41f4-b2b0-629a5df86baf","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:10:02 616","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4945, '2020-08-18 13:10:02.623', 'A040', '7e170d3a-f26f-41f4-b2b0-629a5df86baf', '2020-08-18 13:10:02.622', '2020-08-18 13:10:02.622', '{"Header":{"SessionID":"7e170d3a-f26f-41f4-b2b0-629a5df86baf","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:10:02 616","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:10:02 619"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4946, '2020-08-18 13:10:49.642', 'A001', '47844bcb-5997-46ad-b819-a3a73194479b', NULL, '2020-08-18 13:10:49.636', '{"Header":{"SessionID":"47844bcb-5997-46ad-b819-a3a73194479b","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:10:49 636"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4947, '2020-08-18 13:10:49.689', 'A002', '47844bcb-5997-46ad-b819-a3a73194479b', '2020-08-18 13:10:49.646', '2020-08-18 13:10:49.646', '{"Header":{"SessionID":"47844bcb-5997-46ad-b819-a3a73194479b","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:10:49 636","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:10:49 641"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4948, '2020-08-18 13:10:51.619', 'A039', '562e257b-7ae7-4d6c-b7cd-fae32dcb5974', NULL, '2020-08-18 13:10:51.615', '{"Header":{"SessionID":"562e257b-7ae7-4d6c-b7cd-fae32dcb5974","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:10:51 615","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4949, '2020-08-18 13:10:51.623', 'A040', '562e257b-7ae7-4d6c-b7cd-fae32dcb5974', '2020-08-18 13:10:51.622', '2020-08-18 13:10:51.622', '{"Header":{"SessionID":"562e257b-7ae7-4d6c-b7cd-fae32dcb5974","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:10:51 615","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:10:51 618"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4950, '2020-08-18 13:13:39.636', 'A001', '67ae3636-901b-421e-8d18-a0d9d4adf2dd', NULL, '2020-08-18 13:13:39.624', '{"Header":{"SessionID":"67ae3636-901b-421e-8d18-a0d9d4adf2dd","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:39 624"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4951, '2020-08-18 13:13:44.411', 'A002', '67ae3636-901b-421e-8d18-a0d9d4adf2dd', '2020-08-18 13:13:44.408', '2020-08-18 13:13:44.407', '{"Header":{"SessionID":"67ae3636-901b-421e-8d18-a0d9d4adf2dd","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:39 624","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:39 635"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4952, '2020-08-18 13:13:54.054', 'A023', '1dbf591a-fa4a-40fe-a0c6-d07cd5574f13', NULL, '2020-08-18 13:13:54.048', '{"Header":{"SessionID":"1dbf591a-fa4a-40fe-a0c6-d07cd5574f13","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:54 048"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(4953, '2020-08-18 13:13:54.057', 'A024', '1dbf591a-fa4a-40fe-a0c6-d07cd5574f13', '2020-08-18 13:13:54.055', '2020-08-18 13:13:54.055', '{"Header":{"SessionID":"1dbf591a-fa4a-40fe-a0c6-d07cd5574f13","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:54 048","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:54 053"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4954, '2020-08-18 13:13:54.090', 'A019', '58e71f14-0a09-46ec-bc39-5f1b9344d799', NULL, '2020-08-18 13:13:54.081', '{"Header":{"SessionID":"58e71f14-0a09-46ec-bc39-5f1b9344d799","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:54 081"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(4955, '2020-08-18 13:13:54.095', 'A020', '58e71f14-0a09-46ec-bc39-5f1b9344d799', '2020-08-18 13:13:54.093', '2020-08-18 13:13:54.093', '{"Header":{"SessionID":"58e71f14-0a09-46ec-bc39-5f1b9344d799","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:54 081","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:54 089"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4956, '2020-08-18 13:13:54.117', 'A019', 'bd9c0d65-5cae-46a2-9013-5bff077dd8b9', NULL, '2020-08-18 13:13:54.113', '{"Header":{"SessionID":"bd9c0d65-5cae-46a2-9013-5bff077dd8b9","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:54 113"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(4957, '2020-08-18 13:13:54.121', 'A020', 'bd9c0d65-5cae-46a2-9013-5bff077dd8b9', '2020-08-18 13:13:54.119', '2020-08-18 13:13:54.119', '{"Header":{"SessionID":"bd9c0d65-5cae-46a2-9013-5bff077dd8b9","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:54 113","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:54 115"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4958, '2020-08-18 13:13:54.157', 'A021', '737b0bc8-8299-4c14-800d-4a649b1246b1', NULL, '2020-08-18 13:13:54.133', '{"Header":{"SessionID":"737b0bc8-8299-4c14-800d-4a649b1246b1","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:54 133"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(4959, '2020-08-18 13:13:54.161', 'A022', '737b0bc8-8299-4c14-800d-4a649b1246b1', '2020-08-18 13:13:54.160', '2020-08-18 13:13:54.160', '{"Header":{"SessionID":"737b0bc8-8299-4c14-800d-4a649b1246b1","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:54 133","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:54 157"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4960, '2020-08-18 13:13:56.806', 'A039', 'e68f2c43-082b-438f-8fff-1bf1ecc801c1', NULL, '2020-08-18 13:13:56.798', '{"Header":{"SessionID":"e68f2c43-082b-438f-8fff-1bf1ecc801c1","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:13:56 798","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4961, '2020-08-18 13:13:56.807', 'A040', 'e68f2c43-082b-438f-8fff-1bf1ecc801c1', '2020-08-18 13:13:56.807', '2020-08-18 13:13:56.807', '{"Header":{"SessionID":"e68f2c43-082b-438f-8fff-1bf1ecc801c1","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:13:56 798","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:13:56 801"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4962, '2020-08-18 13:18:54.899', 'A001', 'fc813044-7c2a-49e0-9c13-adff082c5fa4', NULL, '2020-08-18 13:18:54.894', '{"Header":{"SessionID":"fc813044-7c2a-49e0-9c13-adff082c5fa4","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:18:54 894"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4963, '2020-08-18 13:18:54.907', 'A002', 'fc813044-7c2a-49e0-9c13-adff082c5fa4', '2020-08-18 13:18:54.902', '2020-08-18 13:18:54.902', '{"Header":{"SessionID":"fc813044-7c2a-49e0-9c13-adff082c5fa4","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:18:54 894","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:18:54 897"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4964, '2020-08-18 13:18:55.052', 'A023', '2aa676ac-513d-495f-a10f-d1f6155a8a3c', NULL, '2020-08-18 13:18:55.045', '{"Header":{"SessionID":"2aa676ac-513d-495f-a10f-d1f6155a8a3c","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:18:55 045"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(4965, '2020-08-18 13:18:55.055', 'A024', '2aa676ac-513d-495f-a10f-d1f6155a8a3c', '2020-08-18 13:18:55.054', '2020-08-18 13:18:55.054', '{"Header":{"SessionID":"2aa676ac-513d-495f-a10f-d1f6155a8a3c","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:18:55 045","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:18:55 050"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4966, '2020-08-18 13:18:55.085', 'A019', 'b2c2ec57-397b-473a-81db-a48d4aa1e0d4', NULL, '2020-08-18 13:18:55.076', '{"Header":{"SessionID":"b2c2ec57-397b-473a-81db-a48d4aa1e0d4","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:18:55 076"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(4967, '2020-08-18 13:18:55.089', 'A020', 'b2c2ec57-397b-473a-81db-a48d4aa1e0d4', '2020-08-18 13:18:55.087', '2020-08-18 13:18:55.087', '{"Header":{"SessionID":"b2c2ec57-397b-473a-81db-a48d4aa1e0d4","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:18:55 076","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:18:55 083"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4968, '2020-08-18 13:18:55.131', 'A021', 'de09f9e6-0eda-4651-99b0-85e533af5ccd', NULL, '2020-08-18 13:18:55.106', '{"Header":{"SessionID":"de09f9e6-0eda-4651-99b0-85e533af5ccd","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:18:55 106"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(4969, '2020-08-18 13:18:55.135', 'A022', 'de09f9e6-0eda-4651-99b0-85e533af5ccd', '2020-08-18 13:18:55.134', '2020-08-18 13:18:55.134', '{"Header":{"SessionID":"de09f9e6-0eda-4651-99b0-85e533af5ccd","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:18:55 106","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:18:55 130"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4970, '2020-08-18 13:18:55.574', 'A039', 'e51786bc-a45c-48fb-ab7e-ef2baf9d8abd', NULL, '2020-08-18 13:18:55.570', '{"Header":{"SessionID":"e51786bc-a45c-48fb-ab7e-ef2baf9d8abd","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:18:55 570","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4971, '2020-08-18 13:18:55.620', 'A040', 'e51786bc-a45c-48fb-ab7e-ef2baf9d8abd', '2020-08-18 13:18:55.577', '2020-08-18 13:18:55.577', '{"Header":{"SessionID":"e51786bc-a45c-48fb-ab7e-ef2baf9d8abd","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:18:55 570","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:18:55 573"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4972, '2020-08-18 13:24:41.865', 'A001', '6e31bcb6-be2b-410e-b20b-58d7ca9a5be6', NULL, '2020-08-18 13:24:41.858', '{"Header":{"SessionID":"6e31bcb6-be2b-410e-b20b-58d7ca9a5be6","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:24:41 858"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4973, '2020-08-18 13:24:41.920', 'A002', '6e31bcb6-be2b-410e-b20b-58d7ca9a5be6', '2020-08-18 13:24:41.871', '2020-08-18 13:24:41.871', '{"Header":{"SessionID":"6e31bcb6-be2b-410e-b20b-58d7ca9a5be6","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:24:41 858","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:24:41 864"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4974, '2020-08-18 13:24:42.017', 'A023', '0355434c-7d4c-4caf-b2db-849c6b2e3798', NULL, '2020-08-18 13:24:42.011', '{"Header":{"SessionID":"0355434c-7d4c-4caf-b2db-849c6b2e3798","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:24:42 011"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(4975, '2020-08-18 13:24:42.022', 'A024', '0355434c-7d4c-4caf-b2db-849c6b2e3798', '2020-08-18 13:24:42.020', '2020-08-18 13:24:42.020', '{"Header":{"SessionID":"0355434c-7d4c-4caf-b2db-849c6b2e3798","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:24:42 011","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:24:42 016"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4976, '2020-08-18 13:24:42.058', 'A019', '7743e991-cddf-4110-a7f3-59c7f8128973', NULL, '2020-08-18 13:24:42.050', '{"Header":{"SessionID":"7743e991-cddf-4110-a7f3-59c7f8128973","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:24:42 050"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(4977, '2020-08-18 13:24:42.062', 'A020', '7743e991-cddf-4110-a7f3-59c7f8128973', '2020-08-18 13:24:42.061', '2020-08-18 13:24:42.061', '{"Header":{"SessionID":"7743e991-cddf-4110-a7f3-59c7f8128973","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:24:42 050","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:24:42 057"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4978, '2020-08-18 13:24:42.111', 'A021', 'd49ac02d-43e3-43ae-bb8a-27cd1069f574', NULL, '2020-08-18 13:24:42.085', '{"Header":{"SessionID":"d49ac02d-43e3-43ae-bb8a-27cd1069f574","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:24:42 085"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(4979, '2020-08-18 13:24:42.115', 'A022', 'd49ac02d-43e3-43ae-bb8a-27cd1069f574', '2020-08-18 13:24:42.113', '2020-08-18 13:24:42.113', '{"Header":{"SessionID":"d49ac02d-43e3-43ae-bb8a-27cd1069f574","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:24:42 085","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:24:42 109"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4980, '2020-08-18 13:25:16.637', 'A039', '73a4200b-3ca0-4981-b488-4e5814117dce', NULL, '2020-08-18 13:25:16.632', '{"Header":{"SessionID":"73a4200b-3ca0-4981-b488-4e5814117dce","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:25:16 632","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4981, '2020-08-18 13:25:16.642', 'A040', '73a4200b-3ca0-4981-b488-4e5814117dce', '2020-08-18 13:25:16.640', '2020-08-18 13:25:16.640', '{"Header":{"SessionID":"73a4200b-3ca0-4981-b488-4e5814117dce","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:25:16 632","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:25:16 636"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4982, '2020-08-18 13:44:05.772', 'A001', '7af689be-c1c6-46af-b97e-b41a54075a54', NULL, '2020-08-18 13:44:05.767', '{"Header":{"SessionID":"7af689be-c1c6-46af-b97e-b41a54075a54","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:44:05 767"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4983, '2020-08-18 13:44:05.777', 'A002', '7af689be-c1c6-46af-b97e-b41a54075a54', '2020-08-18 13:44:05.775', '2020-08-18 13:44:05.775', '{"Header":{"SessionID":"7af689be-c1c6-46af-b97e-b41a54075a54","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:44:05 767","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:44:05 770"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4984, '2020-08-18 13:44:37.507', 'A001', 'abc022cd-9daf-4de0-af60-d254534264c3', NULL, '2020-08-18 13:44:37.502', '{"Header":{"SessionID":"abc022cd-9daf-4de0-af60-d254534264c3","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:44:37 502"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4985, '2020-08-18 13:44:37.551', 'A002', 'abc022cd-9daf-4de0-af60-d254534264c3', '2020-08-18 13:44:37.509', '2020-08-18 13:44:37.509', '{"Header":{"SessionID":"abc022cd-9daf-4de0-af60-d254534264c3","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:44:37 502","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:44:37 505"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4986, '2020-08-18 13:47:24.628', 'A001', '83083115-ce0d-41e2-b4ce-c8498c26e5d2', NULL, '2020-08-18 13:47:24.622', '{"Header":{"SessionID":"83083115-ce0d-41e2-b4ce-c8498c26e5d2","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:47:24 622"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4987, '2020-08-18 13:47:24.643', 'A002', '83083115-ce0d-41e2-b4ce-c8498c26e5d2', '2020-08-18 13:47:24.633', '2020-08-18 13:47:24.633', '{"Header":{"SessionID":"83083115-ce0d-41e2-b4ce-c8498c26e5d2","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:47:24 622","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:47:24 626"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4988, '2020-08-18 13:47:24.779', 'A023', '2ab65ab4-c8ab-40bf-acf4-282d9626a5b5', NULL, '2020-08-18 13:47:24.773', '{"Header":{"SessionID":"2ab65ab4-c8ab-40bf-acf4-282d9626a5b5","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:47:24 773"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(4989, '2020-08-18 13:47:24.783', 'A024', '2ab65ab4-c8ab-40bf-acf4-282d9626a5b5', '2020-08-18 13:47:24.782', '2020-08-18 13:47:24.782', '{"Header":{"SessionID":"2ab65ab4-c8ab-40bf-acf4-282d9626a5b5","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:47:24 773","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:47:24 778"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4990, '2020-08-18 13:47:24.816', 'A019', '10ef96c1-5eed-4bd5-bb4c-8f4ec09657cb', NULL, '2020-08-18 13:47:24.806', '{"Header":{"SessionID":"10ef96c1-5eed-4bd5-bb4c-8f4ec09657cb","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:47:24 806"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(4991, '2020-08-18 13:47:24.821', 'A020', '10ef96c1-5eed-4bd5-bb4c-8f4ec09657cb', '2020-08-18 13:47:24.820', '2020-08-18 13:47:24.820', '{"Header":{"SessionID":"10ef96c1-5eed-4bd5-bb4c-8f4ec09657cb","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:47:24 806","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:47:24 815"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4992, '2020-08-18 13:47:24.865', 'A021', 'f0d5f11d-0bbe-429a-889f-c438ecbb6e48', NULL, '2020-08-18 13:47:24.839', '{"Header":{"SessionID":"f0d5f11d-0bbe-429a-889f-c438ecbb6e48","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:47:24 839"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(4993, '2020-08-18 13:47:24.869', 'A022', 'f0d5f11d-0bbe-429a-889f-c438ecbb6e48', '2020-08-18 13:47:24.868', '2020-08-18 13:47:24.868', '{"Header":{"SessionID":"f0d5f11d-0bbe-429a-889f-c438ecbb6e48","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:47:24 839","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:47:24 864"},"ResponseInfo":{"Result":"OK"}}', ''),
	(4994, '2020-08-18 13:47:26.137', 'A039', '6fcc25c4-97ff-4f0f-a9b6-f13c9cc23da8', NULL, '2020-08-18 13:47:26.131', '{"Header":{"SessionID":"6fcc25c4-97ff-4f0f-a9b6-f13c9cc23da8","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 13:47:26 131","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4995, '2020-08-18 13:47:26.181', 'A040', '6fcc25c4-97ff-4f0f-a9b6-f13c9cc23da8', '2020-08-18 13:47:26.139', '2020-08-18 13:47:26.139', '{"Header":{"SessionID":"6fcc25c4-97ff-4f0f-a9b6-f13c9cc23da8","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 13:47:26 131","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 13:47:26 135"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(4996, '2020-08-18 14:07:46.795', 'A001', '4440dcef-fdcd-482f-81ed-5817099c91d7', NULL, '2020-08-18 14:07:46.789', '{"Header":{"SessionID":"4440dcef-fdcd-482f-81ed-5817099c91d7","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:07:46 789"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4997, '2020-08-18 14:07:46.837', 'A002', '4440dcef-fdcd-482f-81ed-5817099c91d7', '2020-08-18 14:07:46.796', '2020-08-18 14:07:46.796', '{"Header":{"SessionID":"4440dcef-fdcd-482f-81ed-5817099c91d7","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:07:46 789","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:07:46 793"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4998, '2020-08-18 14:07:46.951', 'A023', 'd3dbcd92-9179-4a28-9b0c-54721b96ab0d', NULL, '2020-08-18 14:07:46.945', '{"Header":{"SessionID":"d3dbcd92-9179-4a28-9b0c-54721b96ab0d","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:07:46 945"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(4999, '2020-08-18 14:07:46.954', 'A024', 'd3dbcd92-9179-4a28-9b0c-54721b96ab0d', '2020-08-18 14:07:46.953', '2020-08-18 14:07:46.953', '{"Header":{"SessionID":"d3dbcd92-9179-4a28-9b0c-54721b96ab0d","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:07:46 945","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:07:46 949"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5000, '2020-08-18 14:07:46.998', 'A019', '1a0b1bd4-077b-46c0-843a-5f50b8675c81', NULL, '2020-08-18 14:07:46.991', '{"Header":{"SessionID":"1a0b1bd4-077b-46c0-843a-5f50b8675c81","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:07:46 991"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5001, '2020-08-18 14:07:47.003', 'A020', '1a0b1bd4-077b-46c0-843a-5f50b8675c81', '2020-08-18 14:07:47.002', '2020-08-18 14:07:47.002', '{"Header":{"SessionID":"1a0b1bd4-077b-46c0-843a-5f50b8675c81","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:07:46 991","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:07:46 997"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5002, '2020-08-18 14:07:47.048', 'A021', 'de683878-147e-42d0-88b5-4fbcc1ea6220', NULL, '2020-08-18 14:07:47.021', '{"Header":{"SessionID":"de683878-147e-42d0-88b5-4fbcc1ea6220","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:07:47 021"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(5003, '2020-08-18 14:07:47.051', 'A022', 'de683878-147e-42d0-88b5-4fbcc1ea6220', '2020-08-18 14:07:47.050', '2020-08-18 14:07:47.050', '{"Header":{"SessionID":"de683878-147e-42d0-88b5-4fbcc1ea6220","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:07:47 021","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:07:47 047"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5004, '2020-08-18 14:11:57.618', 'A001', '1036ca28-1e70-45b5-bdfc-19542676b6c9', NULL, '2020-08-18 14:11:57.611', '{"Header":{"SessionID":"1036ca28-1e70-45b5-bdfc-19542676b6c9","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:11:57 611"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5005, '2020-08-18 14:11:57.662', 'A002', '1036ca28-1e70-45b5-bdfc-19542676b6c9', '2020-08-18 14:11:57.620', '2020-08-18 14:11:57.620', '{"Header":{"SessionID":"1036ca28-1e70-45b5-bdfc-19542676b6c9","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:11:57 611","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:11:57 616"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5006, '2020-08-18 14:11:57.781', 'A023', 'a7390124-6540-423b-a3a4-59f3005b309f', NULL, '2020-08-18 14:11:57.775', '{"Header":{"SessionID":"a7390124-6540-423b-a3a4-59f3005b309f","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:11:57 775"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5007, '2020-08-18 14:11:57.785', 'A024', 'a7390124-6540-423b-a3a4-59f3005b309f', '2020-08-18 14:11:57.783', '2020-08-18 14:11:57.783', '{"Header":{"SessionID":"a7390124-6540-423b-a3a4-59f3005b309f","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:11:57 775","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:11:57 779"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5008, '2020-08-18 14:11:57.816', 'A019', '6abe19b5-1510-4ee3-975e-4af20ac84c4d', NULL, '2020-08-18 14:11:57.807', '{"Header":{"SessionID":"6abe19b5-1510-4ee3-975e-4af20ac84c4d","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:11:57 807"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5009, '2020-08-18 14:11:57.820', 'A020', '6abe19b5-1510-4ee3-975e-4af20ac84c4d', '2020-08-18 14:11:57.819', '2020-08-18 14:11:57.819', '{"Header":{"SessionID":"6abe19b5-1510-4ee3-975e-4af20ac84c4d","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:11:57 807","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:11:57 814"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5010, '2020-08-18 14:11:57.860', 'A021', '62b16294-4739-4cbc-b814-72ac3b8ee0fe', NULL, '2020-08-18 14:11:57.837', '{"Header":{"SessionID":"62b16294-4739-4cbc-b814-72ac3b8ee0fe","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:11:57 837"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp10","SpartName":"关键配件预期寿命10","UseLifetime":"0"},{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"},{"SpartID":"temp5","SpartName":"关键配件预期寿命5","UseLifetime":"0"},{"SpartID":"temp6","SpartName":"关键配件预期寿命6","UseLifetime":"0"},{"SpartID":"temp7","SpartName":"关键配件预期寿命7","UseLifetime":"0"},{"SpartID":"temp8","SpartName":"关键配件预期寿命8","UseLifetime":"0"},{"SpartID":"temp9","SpartName":"关键配件预期寿命9","UseLifetime":"0"}]}}', ''),
	(5011, '2020-08-18 14:11:57.864', 'A022', '62b16294-4739-4cbc-b814-72ac3b8ee0fe', '2020-08-18 14:11:57.863', '2020-08-18 14:11:57.863', '{"Header":{"SessionID":"62b16294-4739-4cbc-b814-72ac3b8ee0fe","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:11:57 837","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:11:57 860"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5012, '2020-08-18 14:11:59.110', 'A039', '6dc7395d-8426-4ae4-889b-ebcbe8f00ce1', NULL, '2020-08-18 14:11:59.106', '{"Header":{"SessionID":"6dc7395d-8426-4ae4-889b-ebcbe8f00ce1","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:11:59 106","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5013, '2020-08-18 14:11:59.112', 'A040', '6dc7395d-8426-4ae4-889b-ebcbe8f00ce1', '2020-08-18 14:11:59.111', '2020-08-18 14:11:59.111', '{"Header":{"SessionID":"6dc7395d-8426-4ae4-889b-ebcbe8f00ce1","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:11:59 106","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:11:59 109"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5014, '2020-08-18 14:54:29.607', 'A001', 'd2226ac0-930a-4eb0-ade3-97b909f269ba', NULL, '2020-08-18 14:54:29.601', '{"Header":{"SessionID":"d2226ac0-930a-4eb0-ade3-97b909f269ba","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:29 601"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5015, '2020-08-18 14:54:29.612', 'A002', 'd2226ac0-930a-4eb0-ade3-97b909f269ba', '2020-08-18 14:54:29.610', '2020-08-18 14:54:29.610', '{"Header":{"SessionID":"d2226ac0-930a-4eb0-ade3-97b909f269ba","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:29 601","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:29 606"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5016, '2020-08-18 14:54:29.818', 'A023', '918506c1-8a11-467f-a0b2-172cf722869a', NULL, '2020-08-18 14:54:29.811', '{"Header":{"SessionID":"918506c1-8a11-467f-a0b2-172cf722869a","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:29 811"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5017, '2020-08-18 14:54:29.822', 'A024', '918506c1-8a11-467f-a0b2-172cf722869a', '2020-08-18 14:54:29.820', '2020-08-18 14:54:29.820', '{"Header":{"SessionID":"918506c1-8a11-467f-a0b2-172cf722869a","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:29 811","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:29 816"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5018, '2020-08-18 14:54:29.853', 'A019', '0a8e00a8-31a2-436e-8529-f297a7b3a699', NULL, '2020-08-18 14:54:29.844', '{"Header":{"SessionID":"0a8e00a8-31a2-436e-8529-f297a7b3a699","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:29 844"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5019, '2020-08-18 14:54:29.856', 'A020', '0a8e00a8-31a2-436e-8529-f297a7b3a699', '2020-08-18 14:54:29.856', '2020-08-18 14:54:29.856', '{"Header":{"SessionID":"0a8e00a8-31a2-436e-8529-f297a7b3a699","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:29 844","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:29 852"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5020, '2020-08-18 14:54:29.887', 'A021', '158ca975-4989-45e3-abc8-7363a08fec1a', NULL, '2020-08-18 14:54:29.875', '{"Header":{"SessionID":"158ca975-4989-45e3-abc8-7363a08fec1a","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:29 875"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5021, '2020-08-18 14:54:29.891', 'A022', '158ca975-4989-45e3-abc8-7363a08fec1a', '2020-08-18 14:54:29.890', '2020-08-18 14:54:29.890', '{"Header":{"SessionID":"158ca975-4989-45e3-abc8-7363a08fec1a","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:29 875","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:29 887"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5022, '2020-08-18 14:54:29.928', 'A045', '3d56a91a-ca62-46ea-9745-e006a392d703', NULL, '2020-08-18 14:54:29.920', '{"Header":{"SessionID":"3d56a91a-ca62-46ea-9745-e006a392d703","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:29 920"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5023, '2020-08-18 14:54:29.932', 'A046', '3d56a91a-ca62-46ea-9745-e006a392d703', '2020-08-18 14:54:29.931', '2020-08-18 14:54:29.931', '{"Header":{"SessionID":"3d56a91a-ca62-46ea-9745-e006a392d703","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:29 920","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:29 927"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5024, '2020-08-18 14:54:30.761', 'A039', '7473fc1b-96c4-4a67-b98a-437c1b6fb764', NULL, '2020-08-18 14:54:30.756', '{"Header":{"SessionID":"7473fc1b-96c4-4a67-b98a-437c1b6fb764","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:54:30 756","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5025, '2020-08-18 14:54:30.763', 'A040', '7473fc1b-96c4-4a67-b98a-437c1b6fb764', '2020-08-18 14:54:30.762', '2020-08-18 14:54:30.762', '{"Header":{"SessionID":"7473fc1b-96c4-4a67-b98a-437c1b6fb764","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:54:30 756","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:54:30 760"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5026, '2020-08-18 14:56:08.385', 'A025', 'ca78849b-ca8f-4151-9d85-593c05028d6f', NULL, '2020-08-18 14:56:08.378', '{"Header":{"SessionID":"ca78849b-ca8f-4151-9d85-593c05028d6f","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:56:08 378"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm018","AlertName":"上料搬运A位有电池（R112），请手动取走并关闭真空阀！","AlertLevel":"A"}]}}', ''),
	(5027, '2020-08-18 14:56:08.388', 'A026', 'ca78849b-ca8f-4151-9d85-593c05028d6f', '2020-08-18 14:56:08.386', '2020-08-18 14:56:08.386', '{"Header":{"SessionID":"ca78849b-ca8f-4151-9d85-593c05028d6f","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:56:08 378","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:56:08 384"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5028, '2020-08-18 14:59:40.099', 'A025', 'ddfac0e5-9021-43d7-9eaa-0f4c1142a11f', NULL, '2020-08-18 14:59:40.097', '{"Header":{"SessionID":"ddfac0e5-9021-43d7-9eaa-0f4c1142a11f","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 14:59:40 097"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm023","AlertName":"备用61106","AlertLevel":"A"}]}}', ''),
	(5029, '2020-08-18 14:59:40.104', 'A026', 'ddfac0e5-9021-43d7-9eaa-0f4c1142a11f', '2020-08-18 14:59:40.102', '2020-08-18 14:59:40.102', '{"Header":{"SessionID":"ddfac0e5-9021-43d7-9eaa-0f4c1142a11f","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 14:59:40 097","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 14:59:40 097"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5030, '2020-08-18 15:02:48.331', 'A001', 'fe644eba-0e34-452c-bfac-68be1a788ceb', NULL, '2020-08-18 15:02:48.326', '{"Header":{"SessionID":"fe644eba-0e34-452c-bfac-68be1a788ceb","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 326"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5031, '2020-08-18 15:02:48.337', 'A002', 'fe644eba-0e34-452c-bfac-68be1a788ceb', '2020-08-18 15:02:48.336', '2020-08-18 15:02:48.336', '{"Header":{"SessionID":"fe644eba-0e34-452c-bfac-68be1a788ceb","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 326","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 329"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5032, '2020-08-18 15:02:48.490', 'A023', 'a99d3b62-58cc-43f2-99d0-09d5fadfd2c0', NULL, '2020-08-18 15:02:48.484', '{"Header":{"SessionID":"a99d3b62-58cc-43f2-99d0-09d5fadfd2c0","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 484"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5033, '2020-08-18 15:02:48.494', 'A024', 'a99d3b62-58cc-43f2-99d0-09d5fadfd2c0', '2020-08-18 15:02:48.493', '2020-08-18 15:02:48.493', '{"Header":{"SessionID":"a99d3b62-58cc-43f2-99d0-09d5fadfd2c0","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 484","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 489"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5034, '2020-08-18 15:02:48.521', 'A019', '35a47769-4b2f-4b12-b854-8c0e47bfea55', NULL, '2020-08-18 15:02:48.513', '{"Header":{"SessionID":"35a47769-4b2f-4b12-b854-8c0e47bfea55","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 513"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5035, '2020-08-18 15:02:48.525', 'A020', '35a47769-4b2f-4b12-b854-8c0e47bfea55', '2020-08-18 15:02:48.524', '2020-08-18 15:02:48.524', '{"Header":{"SessionID":"35a47769-4b2f-4b12-b854-8c0e47bfea55","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 513","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 520"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5036, '2020-08-18 15:02:48.554', 'A021', '908a3629-3029-44a1-a6d5-7175a17da415', NULL, '2020-08-18 15:02:48.542', '{"Header":{"SessionID":"908a3629-3029-44a1-a6d5-7175a17da415","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 542"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5037, '2020-08-18 15:02:48.558', 'A022', '908a3629-3029-44a1-a6d5-7175a17da415', '2020-08-18 15:02:48.556', '2020-08-18 15:02:48.556', '{"Header":{"SessionID":"908a3629-3029-44a1-a6d5-7175a17da415","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 542","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 553"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5038, '2020-08-18 15:02:48.580', 'A025', 'a9f240e5-e1d5-417c-b799-703849af01a4', NULL, '2020-08-18 15:02:48.574', '{"Header":{"SessionID":"a9f240e5-e1d5-417c-b799-703849af01a4","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 574"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5039, '2020-08-18 15:02:48.584', 'A026', 'a9f240e5-e1d5-417c-b799-703849af01a4', '2020-08-18 15:02:48.583', '2020-08-18 15:02:48.583', '{"Header":{"SessionID":"a9f240e5-e1d5-417c-b799-703849af01a4","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 574","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 579"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5040, '2020-08-18 15:02:48.607', 'A045', 'c554f5d9-e01b-428d-b48e-0a3a1a3cb4d4', NULL, '2020-08-18 15:02:48.599', '{"Header":{"SessionID":"c554f5d9-e01b-428d-b48e-0a3a1a3cb4d4","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:48 599"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5041, '2020-08-18 15:02:48.652', 'A046', 'c554f5d9-e01b-428d-b48e-0a3a1a3cb4d4', '2020-08-18 15:02:48.609', '2020-08-18 15:02:48.609', '{"Header":{"SessionID":"c554f5d9-e01b-428d-b48e-0a3a1a3cb4d4","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:48 599","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:48 606"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5042, '2020-08-18 15:02:49.253', 'A039', '41f0b659-8d7d-4778-be19-f36320225ba0', NULL, '2020-08-18 15:02:49.249', '{"Header":{"SessionID":"41f0b659-8d7d-4778-be19-f36320225ba0","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:02:49 249","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5043, '2020-08-18 15:02:49.257', 'A040', '41f0b659-8d7d-4778-be19-f36320225ba0', '2020-08-18 15:02:49.256', '2020-08-18 15:02:49.256', '{"Header":{"SessionID":"41f0b659-8d7d-4778-be19-f36320225ba0","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:02:49 249","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:02:49 252"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5044, '2020-08-18 15:13:36.616', 'A001', 'd0b3c072-de25-4dd9-bec4-5863906f486e', NULL, '2020-08-18 15:13:36.611', '{"Header":{"SessionID":"d0b3c072-de25-4dd9-bec4-5863906f486e","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:36 611"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5045, '2020-08-18 15:13:36.623', 'A002', 'd0b3c072-de25-4dd9-bec4-5863906f486e', '2020-08-18 15:13:36.620', '2020-08-18 15:13:36.620', '{"Header":{"SessionID":"d0b3c072-de25-4dd9-bec4-5863906f486e","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:36 611","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:36 614"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5046, '2020-08-18 15:13:37.362', 'A039', 'bc2a09eb-0090-4472-ad62-ed7e3f260101', NULL, '2020-08-18 15:13:37.358', '{"Header":{"SessionID":"bc2a09eb-0090-4472-ad62-ed7e3f260101","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:37 358","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5047, '2020-08-18 15:13:37.410', 'A040', 'bc2a09eb-0090-4472-ad62-ed7e3f260101', '2020-08-18 15:13:37.365', '2020-08-18 15:13:37.365', '{"Header":{"SessionID":"bc2a09eb-0090-4472-ad62-ed7e3f260101","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:37 358","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:37 361"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5048, '2020-08-18 15:13:38.057', 'A023', '72bed834-c3b1-46ff-8f50-0e9861d3f481', NULL, '2020-08-18 15:13:38.051', '{"Header":{"SessionID":"72bed834-c3b1-46ff-8f50-0e9861d3f481","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:38 051"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5049, '2020-08-18 15:13:38.063', 'A024', '72bed834-c3b1-46ff-8f50-0e9861d3f481', '2020-08-18 15:13:38.061', '2020-08-18 15:13:38.061', '{"Header":{"SessionID":"72bed834-c3b1-46ff-8f50-0e9861d3f481","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:38 051","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:38 056"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5050, '2020-08-18 15:13:38.133', 'A019', '0f6d2141-84a8-478f-b317-002b94024d2b', NULL, '2020-08-18 15:13:38.109', '{"Header":{"SessionID":"0f6d2141-84a8-478f-b317-002b94024d2b","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:38 109"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5051, '2020-08-18 15:13:38.137', 'A020', '0f6d2141-84a8-478f-b317-002b94024d2b', '2020-08-18 15:13:38.136', '2020-08-18 15:13:38.136', '{"Header":{"SessionID":"0f6d2141-84a8-478f-b317-002b94024d2b","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:38 109","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:38 132"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5052, '2020-08-18 15:13:38.161', 'A019', 'cd0c4c4b-188d-492a-a4c8-9a098b9f26b0', NULL, '2020-08-18 15:13:38.154', '{"Header":{"SessionID":"cd0c4c4b-188d-492a-a4c8-9a098b9f26b0","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:38 154"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5053, '2020-08-18 15:13:38.166', 'A020', 'cd0c4c4b-188d-492a-a4c8-9a098b9f26b0', '2020-08-18 15:13:38.164', '2020-08-18 15:13:38.164', '{"Header":{"SessionID":"cd0c4c4b-188d-492a-a4c8-9a098b9f26b0","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:38 154","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:38 160"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5054, '2020-08-18 15:13:38.211', 'A025', '9b29a36c-3d8a-4c78-8424-0b26bac99c0d', NULL, '2020-08-18 15:13:38.205', '{"Header":{"SessionID":"9b29a36c-3d8a-4c78-8424-0b26bac99c0d","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:13:38 205"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5055, '2020-08-18 15:13:38.216', 'A026', '9b29a36c-3d8a-4c78-8424-0b26bac99c0d', '2020-08-18 15:13:38.214', '2020-08-18 15:13:38.214', '{"Header":{"SessionID":"9b29a36c-3d8a-4c78-8424-0b26bac99c0d","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:13:38 205","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:13:38 210"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5056, '2020-08-18 15:15:31.694', 'A001', '25c41d44-fb7a-4bcc-896a-f346d739bcbb', NULL, '2020-08-18 15:15:31.688', '{"Header":{"SessionID":"25c41d44-fb7a-4bcc-896a-f346d739bcbb","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:15:31 688"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5057, '2020-08-18 15:15:31.743', 'A002', '25c41d44-fb7a-4bcc-896a-f346d739bcbb', '2020-08-18 15:15:31.698', '2020-08-18 15:15:31.698', '{"Header":{"SessionID":"25c41d44-fb7a-4bcc-896a-f346d739bcbb","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:15:31 688","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:15:31 692"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5058, '2020-08-18 15:15:33.133', 'A039', '0e2a93f4-c998-4091-b922-cd4543138d61', NULL, '2020-08-18 15:15:33.129', '{"Header":{"SessionID":"0e2a93f4-c998-4091-b922-cd4543138d61","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:15:33 129","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5059, '2020-08-18 15:15:33.135', 'A023', '93d62ff1-5d5e-47fa-9a71-66d963833df6', NULL, '2020-08-18 15:15:33.129', '{"Header":{"SessionID":"93d62ff1-5d5e-47fa-9a71-66d963833df6","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:15:33 129"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5060, '2020-08-18 15:15:33.140', 'A040', '0e2a93f4-c998-4091-b922-cd4543138d61', '2020-08-18 15:15:33.138', '2020-08-18 15:15:33.138', '{"Header":{"SessionID":"0e2a93f4-c998-4091-b922-cd4543138d61","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:15:33 129","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:15:33 132"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5061, '2020-08-18 15:15:33.146', 'A024', '93d62ff1-5d5e-47fa-9a71-66d963833df6', '2020-08-18 15:15:33.145', '2020-08-18 15:15:33.145', '{"Header":{"SessionID":"93d62ff1-5d5e-47fa-9a71-66d963833df6","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:15:33 129","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:15:33 135"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5062, '2020-08-18 15:15:33.220', 'A019', '3ee77442-db4d-41d9-ba13-ccce2eb4102b', NULL, '2020-08-18 15:15:33.196', '{"Header":{"SessionID":"3ee77442-db4d-41d9-ba13-ccce2eb4102b","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:15:33 196"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5063, '2020-08-18 15:15:33.224', 'A020', '3ee77442-db4d-41d9-ba13-ccce2eb4102b', '2020-08-18 15:15:33.222', '2020-08-18 15:15:33.222', '{"Header":{"SessionID":"3ee77442-db4d-41d9-ba13-ccce2eb4102b","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:15:33 196","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:15:33 219"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5064, '2020-08-18 15:15:33.279', 'A025', 'bf1c6a20-8e22-4212-8fcd-065883489588', NULL, '2020-08-18 15:15:33.270', '{"Header":{"SessionID":"bf1c6a20-8e22-4212-8fcd-065883489588","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:15:33 270"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5065, '2020-08-18 15:15:33.283', 'A026', 'bf1c6a20-8e22-4212-8fcd-065883489588', '2020-08-18 15:15:33.281', '2020-08-18 15:15:33.281', '{"Header":{"SessionID":"bf1c6a20-8e22-4212-8fcd-065883489588","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:15:33 270","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:15:33 277"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5066, '2020-08-18 15:22:42.974', 'A019', 'e13379f4-eaed-41a4-adc1-ea866cb68de7', NULL, '2020-08-18 15:22:42.911', '{"Header":{"SessionID":"e13379f4-eaed-41a4-adc1-ea866cb68de7","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:22:42 911"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5067, '2020-08-18 15:22:42.990', 'A020', 'e13379f4-eaed-41a4-adc1-ea866cb68de7', '2020-08-18 15:22:42.930', '2020-08-18 15:22:42.930', '{"Header":{"SessionID":"e13379f4-eaed-41a4-adc1-ea866cb68de7","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:22:42 911","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:22:42 913"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5068, '2020-08-18 15:22:42.991', 'A025', 'fdf21e29-61b9-4225-8586-c7fcde89e126', NULL, '2020-08-18 15:22:42.990', '{"Header":{"SessionID":"fdf21e29-61b9-4225-8586-c7fcde89e126","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:22:42 990"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"}]}}', ''),
	(5069, '2020-08-18 15:22:42.996', 'A026', 'fdf21e29-61b9-4225-8586-c7fcde89e126', '2020-08-18 15:22:42.994', '2020-08-18 15:22:42.994', '{"Header":{"SessionID":"fdf21e29-61b9-4225-8586-c7fcde89e126","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:22:42 990","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:22:42 990"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5070, '2020-08-18 15:22:43.012', 'A045', '2ae08a8d-e638-471b-bbac-4df0b0843d7f', NULL, '2020-08-18 15:22:43.004', '{"Header":{"SessionID":"2ae08a8d-e638-471b-bbac-4df0b0843d7f","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:22:43 004"},"RequestInfo":{"UserInfo":{"UserID":"000777665","UserLevel":"Administrator","UserName":"1234"},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5071, '2020-08-18 15:22:43.015', 'A046', '2ae08a8d-e638-471b-bbac-4df0b0843d7f', '2020-08-18 15:22:43.013', '2020-08-18 15:22:43.013', '{"Header":{"SessionID":"2ae08a8d-e638-471b-bbac-4df0b0843d7f","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:22:43 004","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:22:43 010"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5072, '2020-08-18 15:23:33.239', 'A019', '6676157a-ab50-446e-a71e-528a325cda05', NULL, '2020-08-18 15:23:33.231', '{"Header":{"SessionID":"6676157a-ab50-446e-a71e-528a325cda05","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:23:33 231"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5073, '2020-08-18 15:23:33.243', 'A020', '6676157a-ab50-446e-a71e-528a325cda05', '2020-08-18 15:23:33.242', '2020-08-18 15:23:33.242', '{"Header":{"SessionID":"6676157a-ab50-446e-a71e-528a325cda05","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:23:33 231","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:23:33 238"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5074, '2020-08-18 15:25:33.216', 'A023', '8ae09d95-e5eb-4e7b-8348-ab4e280f8dd7', NULL, '2020-08-18 15:25:33.215', '{"Header":{"SessionID":"8ae09d95-e5eb-4e7b-8348-ab4e280f8dd7","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:25:33 215"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5075, '2020-08-18 15:25:33.223', 'A024', '8ae09d95-e5eb-4e7b-8348-ab4e280f8dd7', '2020-08-18 15:25:33.219', '2020-08-18 15:25:33.219', '{"Header":{"SessionID":"8ae09d95-e5eb-4e7b-8348-ab4e280f8dd7","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:25:33 215","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:25:33 215"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5076, '2020-08-18 15:25:33.232', 'A019', 'f2cd3c68-e8e2-4f1c-8334-665caadf5073', NULL, '2020-08-18 15:25:33.223', '{"Header":{"SessionID":"f2cd3c68-e8e2-4f1c-8334-665caadf5073","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:25:33 223"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5077, '2020-08-18 15:25:33.236', 'A020', 'f2cd3c68-e8e2-4f1c-8334-665caadf5073', '2020-08-18 15:25:33.235', '2020-08-18 15:25:33.235', '{"Header":{"SessionID":"f2cd3c68-e8e2-4f1c-8334-665caadf5073","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:25:33 223","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:25:33 231"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5078, '2020-08-18 15:32:06.520', 'A001', 'db7fb1ed-eb2c-4fe0-a8fb-50d3dfa11f2f', NULL, '2020-08-18 15:32:06.511', '{"Header":{"SessionID":"db7fb1ed-eb2c-4fe0-a8fb-50d3dfa11f2f","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:32:06 511"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5079, '2020-08-18 15:32:06.957', 'A002', 'db7fb1ed-eb2c-4fe0-a8fb-50d3dfa11f2f', '2020-08-18 15:32:06.740', '2020-08-18 15:32:06.740', '{"Header":{"SessionID":"db7fb1ed-eb2c-4fe0-a8fb-50d3dfa11f2f","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:32:06 511","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:32:06 512"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5080, '2020-08-18 15:34:45.055', 'A001', 'fee31428-307a-4cf7-b27a-2ece1e09a1c1', NULL, '2020-08-18 15:34:45.050', '{"Header":{"SessionID":"fee31428-307a-4cf7-b27a-2ece1e09a1c1","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 050"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5081, '2020-08-18 15:34:45.062', 'A002', 'fee31428-307a-4cf7-b27a-2ece1e09a1c1', '2020-08-18 15:34:45.059', '2020-08-18 15:34:45.059', '{"Header":{"SessionID":"fee31428-307a-4cf7-b27a-2ece1e09a1c1","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 050","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 053"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5082, '2020-08-18 15:34:45.231', 'A023', 'e90990b2-b2fc-4135-b71f-2aa5d1aa6a51', NULL, '2020-08-18 15:34:45.225', '{"Header":{"SessionID":"e90990b2-b2fc-4135-b71f-2aa5d1aa6a51","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 225"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5083, '2020-08-18 15:34:45.236', 'A024', 'e90990b2-b2fc-4135-b71f-2aa5d1aa6a51', '2020-08-18 15:34:45.235', '2020-08-18 15:34:45.235', '{"Header":{"SessionID":"e90990b2-b2fc-4135-b71f-2aa5d1aa6a51","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 225","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 231"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5084, '2020-08-18 15:34:45.271', 'A019', 'bd5827f3-8c9d-4733-be7c-9bceee3308e6', NULL, '2020-08-18 15:34:45.263', '{"Header":{"SessionID":"bd5827f3-8c9d-4733-be7c-9bceee3308e6","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 263"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5085, '2020-08-18 15:34:45.275', 'A020', 'bd5827f3-8c9d-4733-be7c-9bceee3308e6', '2020-08-18 15:34:45.274', '2020-08-18 15:34:45.274', '{"Header":{"SessionID":"bd5827f3-8c9d-4733-be7c-9bceee3308e6","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 263","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 270"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5086, '2020-08-18 15:34:45.306', 'A021', '473261b0-1fa4-4ca1-bb0b-9d48465898b8', NULL, '2020-08-18 15:34:45.293', '{"Header":{"SessionID":"473261b0-1fa4-4ca1-bb0b-9d48465898b8","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 293"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5087, '2020-08-18 15:34:45.309', 'A022', '473261b0-1fa4-4ca1-bb0b-9d48465898b8', '2020-08-18 15:34:45.308', '2020-08-18 15:34:45.308', '{"Header":{"SessionID":"473261b0-1fa4-4ca1-bb0b-9d48465898b8","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 293","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 305"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5088, '2020-08-18 15:34:45.333', 'A025', 'cdac2c72-548f-4071-827a-7fa0196414bc', NULL, '2020-08-18 15:34:45.327', '{"Header":{"SessionID":"cdac2c72-548f-4071-827a-7fa0196414bc","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 327"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5089, '2020-08-18 15:34:45.337', 'A026', 'cdac2c72-548f-4071-827a-7fa0196414bc', '2020-08-18 15:34:45.336', '2020-08-18 15:34:45.336', '{"Header":{"SessionID":"cdac2c72-548f-4071-827a-7fa0196414bc","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 327","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 332"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5090, '2020-08-18 15:34:45.360', 'A045', '46de18ee-b295-4846-96ec-1cc93ba465f1', NULL, '2020-08-18 15:34:45.352', '{"Header":{"SessionID":"46de18ee-b295-4846-96ec-1cc93ba465f1","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:45 352"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5091, '2020-08-18 15:34:45.364', 'A046', '46de18ee-b295-4846-96ec-1cc93ba465f1', '2020-08-18 15:34:45.362', '2020-08-18 15:34:45.362', '{"Header":{"SessionID":"46de18ee-b295-4846-96ec-1cc93ba465f1","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:45 352","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:45 359"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5092, '2020-08-18 15:34:46.198', 'A019', '3a9e1767-0e34-4d51-a90d-27027e4881db', NULL, '2020-08-18 15:34:46.195', '{"Header":{"SessionID":"3a9e1767-0e34-4d51-a90d-27027e4881db","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:46 195"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5093, '2020-08-18 15:34:46.202', 'A020', '3a9e1767-0e34-4d51-a90d-27027e4881db', '2020-08-18 15:34:46.201', '2020-08-18 15:34:46.201', '{"Header":{"SessionID":"3a9e1767-0e34-4d51-a90d-27027e4881db","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:46 195","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:46 197"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5094, '2020-08-18 15:34:46.271', 'A039', '8e3f06e9-168c-4067-aac6-09eba8114373', NULL, '2020-08-18 15:34:46.266', '{"Header":{"SessionID":"8e3f06e9-168c-4067-aac6-09eba8114373","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:34:46 266","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5095, '2020-08-18 15:34:46.275', 'A040', '8e3f06e9-168c-4067-aac6-09eba8114373', '2020-08-18 15:34:46.273', '2020-08-18 15:34:46.273', '{"Header":{"SessionID":"8e3f06e9-168c-4067-aac6-09eba8114373","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:34:46 266","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:34:46 269"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5096, '2020-08-18 15:51:34.506', 'A001', '60b9d3c8-80d3-448a-91a4-41ddb20854f9', NULL, '2020-08-18 15:51:34.501', '{"Header":{"SessionID":"60b9d3c8-80d3-448a-91a4-41ddb20854f9","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:51:34 501"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5097, '2020-08-18 15:51:34.522', 'A002', '60b9d3c8-80d3-448a-91a4-41ddb20854f9', '2020-08-18 15:51:34.510', '2020-08-18 15:51:34.510', '{"Header":{"SessionID":"60b9d3c8-80d3-448a-91a4-41ddb20854f9","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:51:34 501","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:51:34 504"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5098, '2020-08-18 15:56:14.112', 'A001', 'fe89f209-9521-44ec-8c15-ad6b4dafd0af', NULL, '2020-08-18 15:56:14.107', '{"Header":{"SessionID":"fe89f209-9521-44ec-8c15-ad6b4dafd0af","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 15:56:14 107"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5099, '2020-08-18 15:56:14.128', 'A002', 'fe89f209-9521-44ec-8c15-ad6b4dafd0af', '2020-08-18 15:56:14.125', '2020-08-18 15:56:14.125', '{"Header":{"SessionID":"fe89f209-9521-44ec-8c15-ad6b4dafd0af","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 15:56:14 107","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 15:56:14 110"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5100, '2020-08-18 16:05:37.183', 'A001', '7a570c64-b45d-460e-bbd6-7bab445a52bc', NULL, '2020-08-18 16:05:37.177', '{"Header":{"SessionID":"7a570c64-b45d-460e-bbd6-7bab445a52bc","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:37 177"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5101, '2020-08-18 16:05:37.190', 'A002', '7a570c64-b45d-460e-bbd6-7bab445a52bc', '2020-08-18 16:05:37.187', '2020-08-18 16:05:37.187', '{"Header":{"SessionID":"7a570c64-b45d-460e-bbd6-7bab445a52bc","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:37 177","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:37 182"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5102, '2020-08-18 16:05:45.358', 'A023', 'dbebc28a-776c-484b-9c5f-8b458e0dae64', NULL, '2020-08-18 16:05:45.351', '{"Header":{"SessionID":"dbebc28a-776c-484b-9c5f-8b458e0dae64","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:45 351"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5103, '2020-08-18 16:05:45.399', 'A019', 'ae8e4785-063f-41c6-87df-6d339117417b', NULL, '2020-08-18 16:05:45.390', '{"Header":{"SessionID":"ae8e4785-063f-41c6-87df-6d339117417b","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:45 390"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5104, '2020-08-18 16:05:45.403', 'A020', 'ae8e4785-063f-41c6-87df-6d339117417b', '2020-08-18 16:05:45.401', '2020-08-18 16:05:45.401', '{"Header":{"SessionID":"ae8e4785-063f-41c6-87df-6d339117417b","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:45 390","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:45 397"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5105, '2020-08-18 16:05:45.409', 'A024', 'dbebc28a-776c-484b-9c5f-8b458e0dae64', '2020-08-18 16:05:45.359', '2020-08-18 16:05:45.359', '{"Header":{"SessionID":"dbebc28a-776c-484b-9c5f-8b458e0dae64","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:45 351","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:45 357"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5106, '2020-08-18 16:05:45.434', 'A021', '1149af53-ff23-4e16-97dc-d10046c90e8a', NULL, '2020-08-18 16:05:45.420', '{"Header":{"SessionID":"1149af53-ff23-4e16-97dc-d10046c90e8a","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:45 420"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5107, '2020-08-18 16:05:45.437', 'A022', '1149af53-ff23-4e16-97dc-d10046c90e8a', '2020-08-18 16:05:45.435', '2020-08-18 16:05:45.435', '{"Header":{"SessionID":"1149af53-ff23-4e16-97dc-d10046c90e8a","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:45 420","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:45 432"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5108, '2020-08-18 16:05:45.461', 'A025', '3c51b4d9-6a1f-4144-8bb8-93b785586a40', NULL, '2020-08-18 16:05:45.455', '{"Header":{"SessionID":"3c51b4d9-6a1f-4144-8bb8-93b785586a40","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:45 455"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5109, '2020-08-18 16:05:45.465', 'A026', '3c51b4d9-6a1f-4144-8bb8-93b785586a40', '2020-08-18 16:05:45.464', '2020-08-18 16:05:45.464', '{"Header":{"SessionID":"3c51b4d9-6a1f-4144-8bb8-93b785586a40","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:45 455","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:45 460"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5110, '2020-08-18 16:05:45.487', 'A045', 'c12899f4-9011-40d2-8d0e-c2c09a90074c', NULL, '2020-08-18 16:05:45.480', '{"Header":{"SessionID":"c12899f4-9011-40d2-8d0e-c2c09a90074c","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:45 480"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5111, '2020-08-18 16:05:45.490', 'A046', 'c12899f4-9011-40d2-8d0e-c2c09a90074c', '2020-08-18 16:05:45.490', '2020-08-18 16:05:45.489', '{"Header":{"SessionID":"c12899f4-9011-40d2-8d0e-c2c09a90074c","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:45 480","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:45 486"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5112, '2020-08-18 16:05:47.907', 'A039', '0b172e45-a7ba-46e7-b1e3-5ad2fbf9c47d', NULL, '2020-08-18 16:05:47.903', '{"Header":{"SessionID":"0b172e45-a7ba-46e7-b1e3-5ad2fbf9c47d","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:05:47 903","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5113, '2020-08-18 16:05:47.910', 'A040', '0b172e45-a7ba-46e7-b1e3-5ad2fbf9c47d', '2020-08-18 16:05:47.909', '2020-08-18 16:05:47.909', '{"Header":{"SessionID":"0b172e45-a7ba-46e7-b1e3-5ad2fbf9c47d","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:05:47 903","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:05:47 906"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5114, '2020-08-18 16:09:14.283', 'A001', 'fb9fda42-0626-4474-8ec0-4823cf5bfd21', NULL, '2020-08-18 16:09:14.267', '{"Header":{"SessionID":"fb9fda42-0626-4474-8ec0-4823cf5bfd21","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 267"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5115, '2020-08-18 16:09:14.289', 'A002', 'fb9fda42-0626-4474-8ec0-4823cf5bfd21', '2020-08-18 16:09:14.286', '2020-08-18 16:09:14.286', '{"Header":{"SessionID":"fb9fda42-0626-4474-8ec0-4823cf5bfd21","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 267","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 281"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5116, '2020-08-18 16:09:14.538', 'A023', '48499ee5-a860-4438-8251-45c47ff78451', NULL, '2020-08-18 16:09:14.532', '{"Header":{"SessionID":"48499ee5-a860-4438-8251-45c47ff78451","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 532"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5117, '2020-08-18 16:09:14.542', 'A024', '48499ee5-a860-4438-8251-45c47ff78451', '2020-08-18 16:09:14.540', '2020-08-18 16:09:14.540', '{"Header":{"SessionID":"48499ee5-a860-4438-8251-45c47ff78451","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 532","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 537"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5118, '2020-08-18 16:09:14.597', 'A019', '88e6e98a-5c6d-488f-b7f5-1b616086ef87', NULL, '2020-08-18 16:09:14.589', '{"Header":{"SessionID":"88e6e98a-5c6d-488f-b7f5-1b616086ef87","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 589"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5119, '2020-08-18 16:09:14.601', 'A020', '88e6e98a-5c6d-488f-b7f5-1b616086ef87', '2020-08-18 16:09:14.600', '2020-08-18 16:09:14.600', '{"Header":{"SessionID":"88e6e98a-5c6d-488f-b7f5-1b616086ef87","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 589","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 596"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5120, '2020-08-18 16:09:14.630', 'A021', '3618a514-b995-439d-b75a-327f13e3ad11', NULL, '2020-08-18 16:09:14.618', '{"Header":{"SessionID":"3618a514-b995-439d-b75a-327f13e3ad11","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 618"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5121, '2020-08-18 16:09:14.634', 'A022', '3618a514-b995-439d-b75a-327f13e3ad11', '2020-08-18 16:09:14.632', '2020-08-18 16:09:14.632', '{"Header":{"SessionID":"3618a514-b995-439d-b75a-327f13e3ad11","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 618","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 629"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5122, '2020-08-18 16:09:14.656', 'A025', '3c6b6053-7e3b-46a5-8e40-67549ef9dcdd', NULL, '2020-08-18 16:09:14.650', '{"Header":{"SessionID":"3c6b6053-7e3b-46a5-8e40-67549ef9dcdd","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 650"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5123, '2020-08-18 16:09:14.661', 'A026', '3c6b6053-7e3b-46a5-8e40-67549ef9dcdd', '2020-08-18 16:09:14.659', '2020-08-18 16:09:14.659', '{"Header":{"SessionID":"3c6b6053-7e3b-46a5-8e40-67549ef9dcdd","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 650","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 655"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5124, '2020-08-18 16:09:14.684', 'A045', 'bf43e156-cda8-443e-aa85-0bc7a1bbb08c', NULL, '2020-08-18 16:09:14.676', '{"Header":{"SessionID":"bf43e156-cda8-443e-aa85-0bc7a1bbb08c","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:14 676"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5125, '2020-08-18 16:09:14.688', 'A046', 'bf43e156-cda8-443e-aa85-0bc7a1bbb08c', '2020-08-18 16:09:14.687', '2020-08-18 16:09:14.687', '{"Header":{"SessionID":"bf43e156-cda8-443e-aa85-0bc7a1bbb08c","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:14 676","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:14 683"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5126, '2020-08-18 16:09:15.664', 'A039', 'f93ece89-c57f-4bfb-8da4-b4d5f88e2fc8', NULL, '2020-08-18 16:09:15.660', '{"Header":{"SessionID":"f93ece89-c57f-4bfb-8da4-b4d5f88e2fc8","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:15 660","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5127, '2020-08-18 16:09:15.668', 'A040', 'f93ece89-c57f-4bfb-8da4-b4d5f88e2fc8', '2020-08-18 16:09:15.667', '2020-08-18 16:09:15.667', '{"Header":{"SessionID":"f93ece89-c57f-4bfb-8da4-b4d5f88e2fc8","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:15 660","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:15 663"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5128, '2020-08-18 16:09:23.978', 'A019', '36020e0b-f526-42b6-a892-cf4281e55778', NULL, '2020-08-18 16:09:23.965', '{"Header":{"SessionID":"36020e0b-f526-42b6-a892-cf4281e55778","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:09:23 965"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5129, '2020-08-18 16:09:23.982', 'A020', '36020e0b-f526-42b6-a892-cf4281e55778', '2020-08-18 16:09:23.981', '2020-08-18 16:09:23.981', '{"Header":{"SessionID":"36020e0b-f526-42b6-a892-cf4281e55778","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:09:23 965","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:09:23 977"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5130, '2020-08-18 16:15:37.308', 'A001', '4e0d5960-2d0f-4e00-b15a-5559772f8b40', NULL, '2020-08-18 16:15:37.304', '{"Header":{"SessionID":"4e0d5960-2d0f-4e00-b15a-5559772f8b40","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:15:37 304"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5131, '2020-08-18 16:15:37.314', 'A002', '4e0d5960-2d0f-4e00-b15a-5559772f8b40', '2020-08-18 16:15:37.313', '2020-08-18 16:15:37.313', '{"Header":{"SessionID":"4e0d5960-2d0f-4e00-b15a-5559772f8b40","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:15:37 304","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:15:37 307"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5132, '2020-08-18 16:15:37.517', 'A023', 'fe0b30f0-3ab6-4983-9540-32082d570c4a', NULL, '2020-08-18 16:15:37.511', '{"Header":{"SessionID":"fe0b30f0-3ab6-4983-9540-32082d570c4a","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:15:37 511"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5133, '2020-08-18 16:15:37.522', 'A024', 'fe0b30f0-3ab6-4983-9540-32082d570c4a', '2020-08-18 16:15:37.520', '2020-08-18 16:15:37.520', '{"Header":{"SessionID":"fe0b30f0-3ab6-4983-9540-32082d570c4a","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:15:37 511","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:15:37 516"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5134, '2020-08-18 16:15:37.586', 'A019', '6806b418-9589-490b-8231-4270fcbd0dba', NULL, '2020-08-18 16:15:37.565', '{"Header":{"SessionID":"6806b418-9589-490b-8231-4270fcbd0dba","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:15:37 565"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5135, '2020-08-18 16:15:37.590', 'A020', '6806b418-9589-490b-8231-4270fcbd0dba', '2020-08-18 16:15:37.588', '2020-08-18 16:15:37.588', '{"Header":{"SessionID":"6806b418-9589-490b-8231-4270fcbd0dba","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:15:37 565","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:15:37 584"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5136, '2020-08-18 16:15:37.648', 'A025', '2af8c01b-16c7-4d71-963f-6562680b5026', NULL, '2020-08-18 16:15:37.641', '{"Header":{"SessionID":"2af8c01b-16c7-4d71-963f-6562680b5026","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:15:37 641"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5137, '2020-08-18 16:15:37.652', 'A026', '2af8c01b-16c7-4d71-963f-6562680b5026', '2020-08-18 16:15:37.651', '2020-08-18 16:15:37.651', '{"Header":{"SessionID":"2af8c01b-16c7-4d71-963f-6562680b5026","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:15:37 641","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:15:37 647"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5138, '2020-08-18 16:15:38.900', 'A039', '36c6feee-783d-4cb6-a7d6-1a3065aae910', NULL, '2020-08-18 16:15:38.896', '{"Header":{"SessionID":"36c6feee-783d-4cb6-a7d6-1a3065aae910","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:15:38 896","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5139, '2020-08-18 16:15:38.951', 'A040', '36c6feee-783d-4cb6-a7d6-1a3065aae910', '2020-08-18 16:15:38.903', '2020-08-18 16:15:38.903', '{"Header":{"SessionID":"36c6feee-783d-4cb6-a7d6-1a3065aae910","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:15:38 896","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:15:38 899"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5140, '2020-08-18 16:24:14.603', 'A001', '56906d68-9fc5-49d4-8bef-0090a2d7c547', NULL, '2020-08-18 16:24:14.598', '{"Header":{"SessionID":"56906d68-9fc5-49d4-8bef-0090a2d7c547","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:24:14 598"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5141, '2020-08-18 16:24:14.651', 'A002', '56906d68-9fc5-49d4-8bef-0090a2d7c547', '2020-08-18 16:24:14.607', '2020-08-18 16:24:14.607', '{"Header":{"SessionID":"56906d68-9fc5-49d4-8bef-0090a2d7c547","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:24:14 598","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:24:14 602"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5142, '2020-08-18 16:24:44.478', 'A001', '09e76f60-e6d5-4221-9415-261861738a24', NULL, '2020-08-18 16:24:44.473', '{"Header":{"SessionID":"09e76f60-e6d5-4221-9415-261861738a24","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:24:44 473"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5143, '2020-08-18 16:24:44.525', 'A002', '09e76f60-e6d5-4221-9415-261861738a24', '2020-08-18 16:24:44.480', '2020-08-18 16:24:44.480', '{"Header":{"SessionID":"09e76f60-e6d5-4221-9415-261861738a24","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:24:44 473","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:24:44 476"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5144, '2020-08-18 16:26:42.610', 'A001', 'a750e43a-8576-4406-ae15-13b03840a6c1', NULL, '2020-08-18 16:26:42.606', '{"Header":{"SessionID":"a750e43a-8576-4406-ae15-13b03840a6c1","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:26:42 606"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5145, '2020-08-18 16:26:42.667', 'A002', 'a750e43a-8576-4406-ae15-13b03840a6c1', '2020-08-18 16:26:42.618', '2020-08-18 16:26:42.618', '{"Header":{"SessionID":"a750e43a-8576-4406-ae15-13b03840a6c1","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:26:42 606","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:26:42 609"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5146, '2020-08-18 16:29:08.039', 'A001', '0cb7dacc-a9e2-4bb6-af4f-d7f1570204e8', NULL, '2020-08-18 16:29:08.034', '{"Header":{"SessionID":"0cb7dacc-a9e2-4bb6-af4f-d7f1570204e8","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:29:08 034"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5147, '2020-08-18 16:29:08.046', 'A002', '0cb7dacc-a9e2-4bb6-af4f-d7f1570204e8', '2020-08-18 16:29:08.042', '2020-08-18 16:29:08.042', '{"Header":{"SessionID":"0cb7dacc-a9e2-4bb6-af4f-d7f1570204e8","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:29:08 034","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:29:08 037"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5148, '2020-08-18 16:48:17.373', 'A001', '5721293e-c0b8-4b1f-9d15-d5057b279997', NULL, '2020-08-18 16:48:17.367', '{"Header":{"SessionID":"5721293e-c0b8-4b1f-9d15-d5057b279997","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 367"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5149, '2020-08-18 16:48:17.432', 'A002', '5721293e-c0b8-4b1f-9d15-d5057b279997', '2020-08-18 16:48:17.379', '2020-08-18 16:48:17.379', '{"Header":{"SessionID":"5721293e-c0b8-4b1f-9d15-d5057b279997","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 367","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 372"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5150, '2020-08-18 16:48:17.532', 'A023', 'ae1d21dd-c016-4b96-b434-2d80f7ea73f7', NULL, '2020-08-18 16:48:17.525', '{"Header":{"SessionID":"ae1d21dd-c016-4b96-b434-2d80f7ea73f7","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 525"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5151, '2020-08-18 16:48:17.536', 'A024', 'ae1d21dd-c016-4b96-b434-2d80f7ea73f7', '2020-08-18 16:48:17.535', '2020-08-18 16:48:17.535', '{"Header":{"SessionID":"ae1d21dd-c016-4b96-b434-2d80f7ea73f7","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 525","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 530"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5152, '2020-08-18 16:48:17.569', 'A019', '0eee9a2d-78c3-4d58-b174-4e75e456c567', NULL, '2020-08-18 16:48:17.561', '{"Header":{"SessionID":"0eee9a2d-78c3-4d58-b174-4e75e456c567","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 561"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5153, '2020-08-18 16:48:17.573', 'A020', '0eee9a2d-78c3-4d58-b174-4e75e456c567', '2020-08-18 16:48:17.572', '2020-08-18 16:48:17.572', '{"Header":{"SessionID":"0eee9a2d-78c3-4d58-b174-4e75e456c567","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 561","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 568"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5154, '2020-08-18 16:48:17.606', 'A021', 'd762f554-4d28-4a8a-94c4-5901bd1f8d7b', NULL, '2020-08-18 16:48:17.591', '{"Header":{"SessionID":"d762f554-4d28-4a8a-94c4-5901bd1f8d7b","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 591"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5155, '2020-08-18 16:48:17.610', 'A022', 'd762f554-4d28-4a8a-94c4-5901bd1f8d7b', '2020-08-18 16:48:17.609', '2020-08-18 16:48:17.609', '{"Header":{"SessionID":"d762f554-4d28-4a8a-94c4-5901bd1f8d7b","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 591","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 605"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5156, '2020-08-18 16:48:17.634', 'A025', 'b38d5cb0-184b-4589-9ac2-87a828d514fd', NULL, '2020-08-18 16:48:17.627', '{"Header":{"SessionID":"b38d5cb0-184b-4589-9ac2-87a828d514fd","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 627"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5157, '2020-08-18 16:48:17.638', 'A026', 'b38d5cb0-184b-4589-9ac2-87a828d514fd', '2020-08-18 16:48:17.637', '2020-08-18 16:48:17.637', '{"Header":{"SessionID":"b38d5cb0-184b-4589-9ac2-87a828d514fd","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 627","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 633"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5158, '2020-08-18 16:48:17.662', 'A045', 'e132374e-1584-4234-bbae-d532e86658c9', NULL, '2020-08-18 16:48:17.654', '{"Header":{"SessionID":"e132374e-1584-4234-bbae-d532e86658c9","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:17 654"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5159, '2020-08-18 16:48:17.666', 'A046', 'e132374e-1584-4234-bbae-d532e86658c9', '2020-08-18 16:48:17.664', '2020-08-18 16:48:17.664', '{"Header":{"SessionID":"e132374e-1584-4234-bbae-d532e86658c9","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:17 654","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:17 660"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5160, '2020-08-18 16:48:18.500', 'A019', 'd291fe0a-f8ac-4d21-bd2a-74432b6e1cd9', NULL, '2020-08-18 16:48:18.497', '{"Header":{"SessionID":"d291fe0a-f8ac-4d21-bd2a-74432b6e1cd9","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:18 497"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5161, '2020-08-18 16:48:18.505', 'A020', 'd291fe0a-f8ac-4d21-bd2a-74432b6e1cd9', '2020-08-18 16:48:18.504', '2020-08-18 16:48:18.504', '{"Header":{"SessionID":"d291fe0a-f8ac-4d21-bd2a-74432b6e1cd9","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:18 497","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:18 499"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5162, '2020-08-18 16:48:19.339', 'A039', 'c1b78a81-6a96-4f46-a112-f5de5e437de0', NULL, '2020-08-18 16:48:19.335', '{"Header":{"SessionID":"c1b78a81-6a96-4f46-a112-f5de5e437de0","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:48:19 335","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5163, '2020-08-18 16:48:19.343', 'A040', 'c1b78a81-6a96-4f46-a112-f5de5e437de0', '2020-08-18 16:48:19.342', '2020-08-18 16:48:19.342', '{"Header":{"SessionID":"c1b78a81-6a96-4f46-a112-f5de5e437de0","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:48:19 335","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:48:19 338"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5164, '2020-08-18 16:49:42.588', 'A019', '855c98ed-8aa6-4c37-aa3c-470f373b4cc1', NULL, '2020-08-18 16:49:42.577', '{"Header":{"SessionID":"855c98ed-8aa6-4c37-aa3c-470f373b4cc1","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:49:42 577"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5165, '2020-08-18 16:49:42.592', 'A020', '855c98ed-8aa6-4c37-aa3c-470f373b4cc1', '2020-08-18 16:49:42.591', '2020-08-18 16:49:42.591', '{"Header":{"SessionID":"855c98ed-8aa6-4c37-aa3c-470f373b4cc1","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:49:42 577","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:49:42 587"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5166, '2020-08-18 16:52:00.459', 'A001', '833d74d8-483a-4c14-b288-5c05c16e567e', NULL, '2020-08-18 16:52:00.452', '{"Header":{"SessionID":"833d74d8-483a-4c14-b288-5c05c16e567e","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:52:00 452"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5167, '2020-08-18 16:52:00.509', 'A002', '833d74d8-483a-4c14-b288-5c05c16e567e', '2020-08-18 16:52:00.462', '2020-08-18 16:52:00.462', '{"Header":{"SessionID":"833d74d8-483a-4c14-b288-5c05c16e567e","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:52:00 452","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:52:00 457"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5168, '2020-08-18 16:52:01.898', 'A039', '58ba8274-9571-4c98-9676-f011580c87c8', NULL, '2020-08-18 16:52:01.893', '{"Header":{"SessionID":"58ba8274-9571-4c98-9676-f011580c87c8","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:52:01 893","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5169, '2020-08-18 16:52:01.902', 'A040', '58ba8274-9571-4c98-9676-f011580c87c8', '2020-08-18 16:52:01.900', '2020-08-18 16:52:01.900', '{"Header":{"SessionID":"58ba8274-9571-4c98-9676-f011580c87c8","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:52:01 893","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:52:01 897"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5170, '2020-08-18 16:54:26.151', 'A001', 'eaac11f2-3ef0-46a0-b014-61cd52293bd9', NULL, '2020-08-18 16:54:26.146', '{"Header":{"SessionID":"eaac11f2-3ef0-46a0-b014-61cd52293bd9","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:54:26 146"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5171, '2020-08-18 16:54:26.201', 'A002', 'eaac11f2-3ef0-46a0-b014-61cd52293bd9', '2020-08-18 16:54:26.154', '2020-08-18 16:54:26.154', '{"Header":{"SessionID":"eaac11f2-3ef0-46a0-b014-61cd52293bd9","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:54:26 146","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:54:26 149"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5172, '2020-08-18 16:54:27.618', 'A039', 'e821d611-690b-4aa2-a7c7-97c821eb3474', NULL, '2020-08-18 16:54:27.612', '{"Header":{"SessionID":"e821d611-690b-4aa2-a7c7-97c821eb3474","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:54:27 612","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5173, '2020-08-18 16:54:27.622', 'A040', 'e821d611-690b-4aa2-a7c7-97c821eb3474', '2020-08-18 16:54:27.620', '2020-08-18 16:54:27.620', '{"Header":{"SessionID":"e821d611-690b-4aa2-a7c7-97c821eb3474","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:54:27 612","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:54:27 616"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5174, '2020-08-18 16:56:33.938', 'A001', '7b2f6def-9a13-4769-8a0c-1e5c9d03e705', NULL, '2020-08-18 16:56:33.933', '{"Header":{"SessionID":"7b2f6def-9a13-4769-8a0c-1e5c9d03e705","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:33 933"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5175, '2020-08-18 16:56:34.002', 'A002', '7b2f6def-9a13-4769-8a0c-1e5c9d03e705', '2020-08-18 16:56:33.942', '2020-08-18 16:56:33.942', '{"Header":{"SessionID":"7b2f6def-9a13-4769-8a0c-1e5c9d03e705","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:33 933","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:33 936"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5176, '2020-08-18 16:56:34.158', 'A023', 'ef064888-36f1-4237-b412-2e9778ed0db3', NULL, '2020-08-18 16:56:34.151', '{"Header":{"SessionID":"ef064888-36f1-4237-b412-2e9778ed0db3","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 151"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5177, '2020-08-18 16:56:34.162', 'A024', 'ef064888-36f1-4237-b412-2e9778ed0db3', '2020-08-18 16:56:34.160', '2020-08-18 16:56:34.160', '{"Header":{"SessionID":"ef064888-36f1-4237-b412-2e9778ed0db3","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 151","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 156"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5178, '2020-08-18 16:56:34.193', 'A019', '72bc7527-451b-4f1e-b600-3ca28346932f', NULL, '2020-08-18 16:56:34.185', '{"Header":{"SessionID":"72bc7527-451b-4f1e-b600-3ca28346932f","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 185"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5179, '2020-08-18 16:56:34.196', 'A020', '72bc7527-451b-4f1e-b600-3ca28346932f', '2020-08-18 16:56:34.196', '2020-08-18 16:56:34.196', '{"Header":{"SessionID":"72bc7527-451b-4f1e-b600-3ca28346932f","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 185","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 192"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5180, '2020-08-18 16:56:34.217', 'A019', '79b076e4-3fdb-4e05-8567-a52ef4cc3832', NULL, '2020-08-18 16:56:34.214', '{"Header":{"SessionID":"79b076e4-3fdb-4e05-8567-a52ef4cc3832","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 214"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5181, '2020-08-18 16:56:34.221', 'A020', '79b076e4-3fdb-4e05-8567-a52ef4cc3832', '2020-08-18 16:56:34.220', '2020-08-18 16:56:34.220', '{"Header":{"SessionID":"79b076e4-3fdb-4e05-8567-a52ef4cc3832","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 214","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 216"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5182, '2020-08-18 16:56:34.249', 'A021', '25acfeb4-6d01-499a-925d-312c2824cc57', NULL, '2020-08-18 16:56:34.237', '{"Header":{"SessionID":"25acfeb4-6d01-499a-925d-312c2824cc57","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 237"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5183, '2020-08-18 16:56:34.253', 'A022', '25acfeb4-6d01-499a-925d-312c2824cc57', '2020-08-18 16:56:34.252', '2020-08-18 16:56:34.252', '{"Header":{"SessionID":"25acfeb4-6d01-499a-925d-312c2824cc57","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 237","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 249"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5184, '2020-08-18 16:56:34.278', 'A025', 'a36cf923-d93d-40fd-8d86-e1fc082f3c39', NULL, '2020-08-18 16:56:34.272', '{"Header":{"SessionID":"a36cf923-d93d-40fd-8d86-e1fc082f3c39","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 272"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5185, '2020-08-18 16:56:34.282', 'A026', 'a36cf923-d93d-40fd-8d86-e1fc082f3c39', '2020-08-18 16:56:34.281', '2020-08-18 16:56:34.281', '{"Header":{"SessionID":"a36cf923-d93d-40fd-8d86-e1fc082f3c39","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 272","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 277"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5186, '2020-08-18 16:56:34.303', 'A045', 'cf53ebac-b19d-461b-84e3-5dba96228b27', NULL, '2020-08-18 16:56:34.296', '{"Header":{"SessionID":"cf53ebac-b19d-461b-84e3-5dba96228b27","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 296"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5187, '2020-08-18 16:56:34.307', 'A046', 'cf53ebac-b19d-461b-84e3-5dba96228b27', '2020-08-18 16:56:34.306', '2020-08-18 16:56:34.306', '{"Header":{"SessionID":"cf53ebac-b19d-461b-84e3-5dba96228b27","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 296","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 302"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5188, '2020-08-18 16:56:34.440', 'A039', '20512026-0b2b-45ab-a1cb-3cd91422a4f8', NULL, '2020-08-18 16:56:34.435', '{"Header":{"SessionID":"20512026-0b2b-45ab-a1cb-3cd91422a4f8","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 16:56:34 435","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5189, '2020-08-18 16:56:34.443', 'A040', '20512026-0b2b-45ab-a1cb-3cd91422a4f8', '2020-08-18 16:56:34.442', '2020-08-18 16:56:34.442', '{"Header":{"SessionID":"20512026-0b2b-45ab-a1cb-3cd91422a4f8","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 16:56:34 435","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 16:56:34 438"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5190, '2020-08-18 17:01:19.796', 'A001', 'b5d84994-a972-4ad8-b49b-b2e254c2339c', NULL, '2020-08-18 17:01:19.791', '{"Header":{"SessionID":"b5d84994-a972-4ad8-b49b-b2e254c2339c","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:19 791"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(5191, '2020-08-18 17:01:19.802', 'A002', 'b5d84994-a972-4ad8-b49b-b2e254c2339c', '2020-08-18 17:01:19.800', '2020-08-18 17:01:19.800', '{"Header":{"SessionID":"b5d84994-a972-4ad8-b49b-b2e254c2339c","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:19 791","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:19 794"},"ResponseInfo":{"Result": "OK"}}', ''),
	(5192, '2020-08-18 17:01:19.975', 'A023', '56a25ff0-8f21-4515-85e4-dbe0affeae8c', NULL, '2020-08-18 17:01:19.967', '{"Header":{"SessionID":"56a25ff0-8f21-4515-85e4-dbe0affeae8c","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:19 967"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5193, '2020-08-18 17:01:19.980', 'A024', '56a25ff0-8f21-4515-85e4-dbe0affeae8c', '2020-08-18 17:01:19.978', '2020-08-18 17:01:19.978', '{"Header":{"SessionID":"56a25ff0-8f21-4515-85e4-dbe0affeae8c","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:19 967","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:19 973"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5194, '2020-08-18 17:01:20.009', 'A019', 'aa9cf704-f86f-4d24-8349-6fe623a0394c', NULL, '2020-08-18 17:01:19.998', '{"Header":{"SessionID":"aa9cf704-f86f-4d24-8349-6fe623a0394c","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:19 998"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5195, '2020-08-18 17:01:20.014', 'A020', 'aa9cf704-f86f-4d24-8349-6fe623a0394c', '2020-08-18 17:01:20.012', '2020-08-18 17:01:20.012', '{"Header":{"SessionID":"aa9cf704-f86f-4d24-8349-6fe623a0394c","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:19 998","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:20 008"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5196, '2020-08-18 17:01:20.045', 'A021', 'c7f58e88-a1ad-40f1-bd90-b85f81ed41c8', NULL, '2020-08-18 17:01:20.030', '{"Header":{"SessionID":"c7f58e88-a1ad-40f1-bd90-b85f81ed41c8","FunctionID":"A021","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:20 030"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"temp2","SpartName":"关键配件预期寿命2","UseLifetime":"0"},{"SpartID":"temp3","SpartName":"关键配件预期寿命3","UseLifetime":"0"},{"SpartID":"temp4","SpartName":"关键配件预期寿命4","UseLifetime":"0"}]}}', ''),
	(5197, '2020-08-18 17:01:20.049', 'A022', 'c7f58e88-a1ad-40f1-bd90-b85f81ed41c8', '2020-08-18 17:01:20.047', '2020-08-18 17:01:20.047', '{"Header":{"SessionID":"c7f58e88-a1ad-40f1-bd90-b85f81ed41c8","FunctionID":"A022","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:20 030","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:20 043"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5198, '2020-08-18 17:01:20.072', 'A025', '0ffde468-4c4e-4692-89a1-5e461830af02', NULL, '2020-08-18 17:01:20.066', '{"Header":{"SessionID":"0ffde468-4c4e-4692-89a1-5e461830af02","FunctionID":"A025","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:20 066"},"RequestInfo":{"ResourceAlertInfo":[{"AlertCode":"Alarm015","AlertName":"下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！","AlertLevel":"A"},{"AlertCode":"Alarm002","AlertName":"备用61001","AlertLevel":"A"},{"AlertCode":"Alarm007","AlertName":"左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！","AlertLevel":"A"}]}}', ''),
	(5199, '2020-08-18 17:01:20.076', 'A026', '0ffde468-4c4e-4692-89a1-5e461830af02', '2020-08-18 17:01:20.075', '2020-08-18 17:01:20.075', '{"Header":{"SessionID":"0ffde468-4c4e-4692-89a1-5e461830af02","FunctionID":"A026","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:20 066","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:20 071"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5200, '2020-08-18 17:01:20.100', 'A045', '1167cc81-7514-4899-bc50-3d97754640b4', NULL, '2020-08-18 17:01:20.093', '{"Header":{"SessionID":"1167cc81-7514-4899-bc50-3d97754640b4","FunctionID":"A045","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:20 093"},"RequestInfo":{"UserInfo":{"UserID":"","UserLevel":"","UserName":""},"EquParam":[{"ParamID":"50819","ParamDesc":"左滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50820","ParamDesc":"右滴胶枪温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50950","ParamDesc":"左一折热压上烫头温度(℃)","OldParamValue":"","NewParamValue":"0"},{"ParamID":"50951","ParamDesc":"左一折热压下烫头温度(℃)","OldParamValue":"","NewParamValue":"0"}]}}', ''),
	(5201, '2020-08-18 17:01:20.104', 'A046', '1167cc81-7514-4899-bc50-3d97754640b4', '2020-08-18 17:01:20.103', '2020-08-18 17:01:20.103', '{"Header":{"SessionID":"1167cc81-7514-4899-bc50-3d97754640b4","FunctionID":"A046","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:20 093","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:20 100"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5202, '2020-08-18 17:01:20.420', 'A039', '2dbc03e1-fe60-461b-8de3-beb7e97e4370', NULL, '2020-08-18 17:01:20.415', '{"Header":{"SessionID":"2dbc03e1-fe60-461b-8de3-beb7e97e4370","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:20 415","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(5203, '2020-08-18 17:01:20.423', 'A040', '2dbc03e1-fe60-461b-8de3-beb7e97e4370', '2020-08-18 17:01:20.422', '2020-08-18 17:01:20.422', '{"Header":{"SessionID":"2dbc03e1-fe60-461b-8de3-beb7e97e4370","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:20 415","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:20 418"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', ''),
	(5204, '2020-08-18 17:01:32.084', 'A019', '30ae5168-45a5-4a18-a3e9-1516e29e7e2b', NULL, '2020-08-18 17:01:32.073', '{"Header":{"SessionID":"30ae5168-45a5-4a18-a3e9-1516e29e7e2b","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:01:32 073"},"RequestInfo":{"ParentEQStateCode":"","AndonState":"","ChildEQ":[],"Quantity":""}}', ''),
	(5205, '2020-08-18 17:01:32.088', 'A020', '30ae5168-45a5-4a18-a3e9-1516e29e7e2b', '2020-08-18 17:01:32.086', '2020-08-18 17:01:32.086', '{"Header":{"SessionID":"30ae5168-45a5-4a18-a3e9-1516e29e7e2b","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:01:32 073","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:01:32 082"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5206, '2020-08-18 17:02:19.664', 'A019', 'bad80c00-623d-4383-874c-e04e5630ecfa', NULL, '2020-08-18 17:02:19.661', '{"Header":{"SessionID":"bad80c00-623d-4383-874c-e04e5630ecfa","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:02:19 661"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5207, '2020-08-18 17:02:19.668', 'A020', 'bad80c00-623d-4383-874c-e04e5630ecfa', '2020-08-18 17:02:19.667', '2020-08-18 17:02:19.667', '{"Header":{"SessionID":"bad80c00-623d-4383-874c-e04e5630ecfa","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:02:19 661","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:02:19 663"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5208, '2020-08-18 17:11:20.009', 'A023', '101c2c29-721e-4675-95e9-5adefcfbf156', NULL, '2020-08-18 17:11:20.007', '{"Header":{"SessionID":"101c2c29-721e-4675-95e9-5adefcfbf156","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:11:20 007"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(5209, '2020-08-18 17:11:20.011', 'A024', '101c2c29-721e-4675-95e9-5adefcfbf156', '2020-08-18 17:11:20.009', '2020-08-18 17:11:20.009', '{"Header":{"SessionID":"101c2c29-721e-4675-95e9-5adefcfbf156","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:11:20 007","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:11:20 007"},"ResponseInfo":{"Result":"OK"}}', ''),
	(5210, '2020-08-18 17:11:20.045', 'A019', '8c54a020-a737-4f62-8cb6-cf3872619b85', NULL, '2020-08-18 17:11:20.041', '{"Header":{"SessionID":"8c54a020-a737-4f62-8cb6-cf3872619b85","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:11:20 041"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(5211, '2020-08-18 17:11:20.049', 'A020', '8c54a020-a737-4f62-8cb6-cf3872619b85', '2020-08-18 17:11:20.047', '2020-08-18 17:11:20.047', '{"Header":{"SessionID":"8c54a020-a737-4f62-8cb6-cf3872619b85","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:11:20 041","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:11:20 043"},"ResponseInfo":{"Result":"OK"}}', '');
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` ENABLE KEYS */;

-- Dumping structure for table ptf.ngcode
CREATE TABLE IF NOT EXISTS `ngcode` (
  `NGCode` varchar(50) NOT NULL COMMENT 'NG代码',
  `NGreason` varchar(50) NOT NULL COMMENT 'NG原因',
  PRIMARY KEY (`NGCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='NGcode原因表';

-- Dumping data for table ptf.ngcode: ~2 rows (大约)
/*!40000 ALTER TABLE `ngcode` DISABLE KEYS */;
INSERT INTO `ngcode` (`NGCode`, `NGreason`) VALUES
	('E-001', '电池超温'),
	('OK', 'OK');
/*!40000 ALTER TABLE `ngcode` ENABLE KEYS */;

-- Dumping structure for table ptf.offline_mes_interface_upload_buffer_datas
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

-- Dumping data for table ptf.offline_mes_interface_upload_buffer_datas: ~0 rows (大约)
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` DISABLE KEYS */;
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` ENABLE KEYS */;

-- Dumping structure for table ptf.permissions
CREATE TABLE IF NOT EXISTS `permissions` (
  `PermissionId` int(11) NOT NULL COMMENT '主键',
  `PermissionName` varchar(20) NOT NULL COMMENT '菜单名称',
  `PermissionCode` varchar(50) DEFAULT NULL COMMENT '菜单编码',
  `Uri` varchar(200) DEFAULT NULL,
  `Remark` varchar(50) DEFAULT NULL COMMENT '备注',
  `DisplayOrder` int(11) NOT NULL COMMENT '显示顺序',
  `ParentId` int(11) DEFAULT NULL COMMENT '父菜单ID',
  `Depth` int(11) NOT NULL COMMENT '层级',
  PRIMARY KEY (`PermissionId`),
  UNIQUE KEY `PermissionCode` (`PermissionCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.permissions: ~46 rows (大约)
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
	(107, '运行日志查询', 'LogQuery.SoftwareLogReport', 'pack://application:,,,/ATL.UI;component/LogQuery/SoftwareLogReport.xaml', NULL, 7, 100, 2),
	(108, '电池生产记录查询', 'LogQuery.ProductionData', 'pack://application:,,,/ATL.UI;component/LogQuery/ProductionData.xaml', NULL, 6, 100, 2),
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

-- Dumping structure for table ptf.plc_area_config
CREATE TABLE IF NOT EXISTS `plc_area_config` (
  `areaName` varchar(50) NOT NULL COMMENT '区域名称',
  `bitLength` int(11) DEFAULT NULL COMMENT '区域位长度',
  `brand` varchar(50) DEFAULT NULL COMMENT '品牌',
  PRIMARY KEY (`areaName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC软元件区域配置';

-- Dumping data for table ptf.plc_area_config: ~9 rows (大约)
/*!40000 ALTER TABLE `plc_area_config` DISABLE KEYS */;
INSERT INTO `plc_area_config` (`areaName`, `bitLength`, `brand`) VALUES
	('A', 16, 'omron'),
	('B', 8, 'Beckoff'),
	('C', 16, 'omron CIO区域'),
	('D', 16, 'melsec,keyence,Panasonic,omron'),
	('DB', 8, 'siemens'),
	('H', 16, 'omron'),
	('M', 1, 'melsec,siemens,keyence,Panasonic'),
	('R', 16, 'keyence,Panasonic'),
	('W', 16, 'omron');
/*!40000 ALTER TABLE `plc_area_config` ENABLE KEYS */;

-- Dumping structure for table ptf.plc_brand
CREATE TABLE IF NOT EXISTS `plc_brand` (
  `brand` varchar(50) NOT NULL,
  PRIMARY KEY (`brand`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC品牌';

-- Dumping data for table ptf.plc_brand: ~7 rows (大约)
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

-- Dumping structure for table ptf.plc_config
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

-- Dumping data for table ptf.plc_config: ~0 rows (大约)
/*!40000 ALTER TABLE `plc_config` DISABLE KEYS */;
INSERT INTO `plc_config` (`plc_ID`, `plcName`, `deviceName`, `address`, `address_para`, `ProtocolName`, `remark`, `enabled`) VALUES
	(1, 'PLC1', 'FEF', '5.12.244.196', '801', 'ADS_D', NULL, 1);
/*!40000 ALTER TABLE `plc_config` ENABLE KEYS */;

-- Dumping structure for table ptf.plc_interactive_invariables
CREATE TABLE IF NOT EXISTS `plc_interactive_invariables` (
  `did` int(11) NOT NULL AUTO_INCREMENT,
  `variableID` int(11) NOT NULL,
  `orderID` int(11) NOT NULL COMMENT '排序显示',
  PRIMARY KEY (`did`),
  KEY `FK_plc_interactive_invariables_user_fefine_variable` (`variableID`),
  KEY `orderID` (`orderID`),
  CONSTRAINT `FK_plc_interactive_invariables_user_define_variable` FOREIGN KEY (`variableID`) REFERENCES `user_define_variable` (`variableID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='上位机-PLC交互变量集合';

-- Dumping data for table ptf.plc_interactive_invariables: ~6 rows (大约)
/*!40000 ALTER TABLE `plc_interactive_invariables` DISABLE KEYS */;
INSERT INTO `plc_interactive_invariables` (`did`, `variableID`, `orderID`) VALUES
	(1, 46, 1),
	(2, 47, 2),
	(3, 48, 3),
	(4, 49, 4),
	(5, 50, 5),
	(6, 51, 6);
/*!40000 ALTER TABLE `plc_interactive_invariables` ENABLE KEYS */;

-- Dumping structure for table ptf.plc_protocol
CREATE TABLE IF NOT EXISTS `plc_protocol` (
  `ProtocolName` varchar(50) NOT NULL COMMENT '通信协议',
  `model` varchar(100) DEFAULT NULL COMMENT '系列',
  `remark` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`ProtocolName`),
  KEY `ProtocolName` (`ProtocolName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC通信协议';

-- Dumping data for table ptf.plc_protocol: ~22 rows (大约)
/*!40000 ALTER TABLE `plc_protocol` DISABLE KEYS */;
INSERT INTO `plc_protocol` (`ProtocolName`, `model`, `remark`) VALUES
	('ADS', 'Beckhoff', '上位机只读写一个结构体，结构体的成员必须与BeckhoffTwinCAT命名空间里RWstruct结构体里的成员一致'),
	('ADS_D', 'Beckhoff', '上位机只读写一个结构体，结构体的成员必须与BeckhoffTwinCAT命名空间里RWstruct结构体里的成员一致'),
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

-- Dumping structure for table ptf.plc_rw_config
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
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8 COMMENT='PLC读写区域设置';

-- Dumping data for table ptf.plc_rw_config: ~3 rows (大约)
/*!40000 ALTER TABLE `plc_rw_config` DISABLE KEYS */;
INSERT INTO `plc_rw_config` (`plc_rw_area_did`, `name`, `plcID`, `areaName`, `startAddress`, `length`, `rw`, `cycle`, `lastTime`, `enabled`) VALUES
	(3, 'input监控地址', 1, 'D', '2000', 100, 'W', 1000, '2019-10-21 09:46:00', 1),
	(4, '设备报警数据', 1, 'D', '1000', 5, 'R', 1000, '2019-09-19 16:50:00', 1),
	(14, 'input监控地址', 1, 'D', '1010', 100, 'R', 1000, '2019-10-21 09:46:00', 1),
	(21, 'NotRW', 1, NULL, NULL, 0, 'NotRW', 0, '2020-02-22 16:55:57', 1);
/*!40000 ALTER TABLE `plc_rw_config` ENABLE KEYS */;

-- Dumping structure for table ptf.plc_rw_field
CREATE TABLE IF NOT EXISTS `plc_rw_field` (
  `rw` varchar(20) NOT NULL COMMENT '设置读或写',
  `remark` varchar(20) NOT NULL COMMENT '备注',
  PRIMARY KEY (`rw`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC操作选择';

-- Dumping data for table ptf.plc_rw_field: ~3 rows (大约)
/*!40000 ALTER TABLE `plc_rw_field` DISABLE KEYS */;
INSERT INTO `plc_rw_field` (`rw`, `remark`) VALUES
	('NotRW', '非周期性读写'),
	('R', '读持续取区域'),
	('W', '持续写入区域');
/*!40000 ALTER TABLE `plc_rw_field` ENABLE KEYS */;

-- Dumping structure for table ptf.probably
CREATE TABLE IF NOT EXISTS `probably` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `plc_ID` int(11) DEFAULT NULL COMMENT '绑定设备名称',
  `CurrYield` int(11) NOT NULL DEFAULT 0,
  `UserName` varchar(50) NOT NULL COMMENT '当前登录帐号',
  `OKCount` int(11) NOT NULL DEFAULT 0,
  `NGCount` int(11) DEFAULT 0,
  `isBaking` varchar(50) NOT NULL DEFAULT 'false' COMMENT 'true表示是baking设备，false表示不是',
  `PPM` int(11) NOT NULL DEFAULT 0,
  `Model` varchar(50) DEFAULT NULL COMMENT '当前生产型号',
  `InAllCount` int(11) NOT NULL DEFAULT 0,
  `InOKallCount` int(11) NOT NULL DEFAULT 0,
  `OutAllCount` int(11) NOT NULL DEFAULT 0,
  `NG1Count` int(11) NOT NULL DEFAULT 0,
  `NG2Count` int(11) NOT NULL DEFAULT 0,
  `NG3Count` int(11) NOT NULL DEFAULT 0,
  `NG4Count` int(11) NOT NULL DEFAULT 0,
  `NG5Count` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `FK_probably_plc_config` (`plc_ID`),
  CONSTRAINT `FK_probably_plc_config` FOREIGN KEY (`plc_ID`) REFERENCES `plc_config` (`plc_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=59 DEFAULT CHARSET=utf8 COMMENT='当前生产概况数据表\r\nisBaking字段表示是否为Baking设备，如果是，值为1，则每个炉子一行数据\r\nInAllCount表示来料总数量，InOKallCount表示来料可拿来生产的总数量(扣减了marking不良等)\r\nOutAllCount表示当前已经下料的总数量，NG1Count表示不良品1数量，NG2Count表示不良品2数量';

-- Dumping data for table ptf.probably: ~0 rows (大约)
/*!40000 ALTER TABLE `probably` DISABLE KEYS */;
INSERT INTO `probably` (`id`, `plc_ID`, `CurrYield`, `UserName`, `OKCount`, `NGCount`, `isBaking`, `PPM`, `Model`, `InAllCount`, `InOKallCount`, `OutAllCount`, `NG1Count`, `NG2Count`, `NG3Count`, `NG4Count`, `NG5Count`) VALUES
	(58, 1, 89, 'SuperAdmin', 89, 11, 'false', 23, 'M20', 90, 89, 80, 2, 1, 5, 1, 2);
/*!40000 ALTER TABLE `probably` ENABLE KEYS */;

-- Dumping structure for table ptf.production_data
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

-- Dumping data for table ptf.production_data: ~0 rows (大约)
/*!40000 ALTER TABLE `production_data` DISABLE KEYS */;
/*!40000 ALTER TABLE `production_data` ENABLE KEYS */;

-- Dumping structure for table ptf.production_ngcode_statistics
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

-- Dumping data for table ptf.production_ngcode_statistics: ~0 rows (大约)
/*!40000 ALTER TABLE `production_ngcode_statistics` DISABLE KEYS */;
INSERT INTO `production_ngcode_statistics` (`id`, `model`, `MCCollectDDate`, `ME`, `NGCode`, `Count`) VALUES
	(1, 'M20', '2019-07-20 10:04:33', 'M', 'E-001', 3566);
/*!40000 ALTER TABLE `production_ngcode_statistics` ENABLE KEYS */;

-- Dumping structure for table ptf.production_statistics
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

-- Dumping data for table ptf.production_statistics: ~30 rows (大约)
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

-- Dumping structure for table ptf.quickwearparthistoryinfo
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

-- Dumping data for table ptf.quickwearparthistoryinfo: ~2 rows (大约)
/*!40000 ALTER TABLE `quickwearparthistoryinfo` DISABLE KEYS */;
INSERT INTO `quickwearparthistoryinfo` (`ID`, `EquipmentID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `UsedLife`, `PercentWarning`, `ParamValueRatio`, `StartDate`, `EndDate`, `StartUser`, `EndUser`) VALUES
	(1, 'Baking001', '00022', '气缸1', 'Float', 1000, '200.20', 0.95, 1.00, '2019-07-22 10:32:19', '2019-08-22 10:32:19', 'admin1', 'admin2'),
	(2, 'Baking002', '00022', '同步带', 'Float', 1000, '100.35', 0.90, 1.00, '2019-09-22 10:32:19', '2019-10-02 10:32:19', 'admin1', 'admin2');
/*!40000 ALTER TABLE `quickwearparthistoryinfo` ENABLE KEYS */;

-- Dumping structure for table ptf.roles
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.roles: ~4 rows (大约)
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` (`roleId`, `roleName`, `MesUserLevel`, `UserLevelPLCValue`, `permissionCodes`, `createTime`, `modifyTime`, `remark`) VALUES
	(0, 'Development Authority', 'Administrator', 408, 'ALL', '2019-07-02 11:04:39', '2020-08-17 21:57:27', '超级管理员,禁止删除'),
	(1, 'Maintain Authority', 'Maintainer', 307, 'ALL', '2019-07-02 11:04:39', '2020-08-17 21:57:27', '系统管理员'),
	(2, 'Operator Authority', 'Operator', 206, 'UserManager,UserManager.Personal,UserManager.User', '2019-07-09 09:50:00', '2020-08-17 21:57:27', 'ME人员'),
	(10, 'Guest Authority', 'Guest', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,MES,MES.MESweb,Maintain,DataWareHouse,SystemSetting,LogQuery,UserManager,UserManager.Login,DeviceControl', '2019-07-02 11:04:39', '2020-08-17 21:57:27', '尚未登陆');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;

-- Dumping structure for table ptf.server_backpara
CREATE TABLE IF NOT EXISTS `server_backpara` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `backupServerIpAdr` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用MES服务端IP地址',
  `backupServerUdpPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用MES服务端udp接收端口号',
  `backupLocalUdpSendPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机udp发送端口',
  `backupLocalUdpRecvPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机tcp接收端口号',
  `backupLocalTcpPortNo` varchar(50) NOT NULL DEFAULT '0' COMMENT '备用本机udp接收端口号',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.server_backpara: ~3 rows (大约)
/*!40000 ALTER TABLE `server_backpara` DISABLE KEYS */;
INSERT INTO `server_backpara` (`ID`, `backupServerIpAdr`, `backupServerUdpPortNo`, `backupLocalUdpSendPortNo`, `backupLocalUdpRecvPortNo`, `backupLocalTcpPortNo`) VALUES
	(1, '127.0.0.1', '50002', '61002', '62000', '60003'),
	(2, '127.0.0.2', '50000', '61001', '61000', '60001'),
	(4, '127.0.0.3', '50000', '61001', '61000', '60001');
/*!40000 ALTER TABLE `server_backpara` ENABLE KEYS */;

-- Dumping structure for table ptf.users
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
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.users: ~4 rows (大约)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`userId`, `userName`, `password`, `name`, `roleId`, `createTime`, `gender`, `LastLoginTime`, `remark`) VALUES
	(1, 'admin', 'admin', 'admin', 1, '2017-03-23 21:05:21', 1, '2019-07-09 08:20:38', NULL),
	(2, 'Guest', 'Guest', 'Guest', 10, '2017-03-23 21:05:21', 1, '2020-08-18 17:01:17', NULL),
	(3, 'chen', 'chen', 'Michal', 2, '0001-01-01 00:00:00', 0, NULL, NULL),
	(4, 'SuperAdmin', 'SuperAdmin', 'SuperAdmin', 0, '2019-07-08 13:22:06', 0, '2020-08-17 22:00:51', NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

-- Dumping structure for table ptf.user_define_variable
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
) ENGINE=InnoDB AUTO_INCREMENT=168 DEFAULT CHARSET=utf8 COMMENT='用户定义变量表。variableTypeID等于1表示为系统变量，系统变量与PLC变量，不是在同一个UI界面上显示的。\r\n系统变量不会与PLC地址绑定。非系统变量可能会与PLC地址绑定关联，用于接口数据上传。\r\n每个MES ID的变量关联的plc_rw_area_did都必须是Ｒ读类型的．MES ID的变量属于OUTPUT参数，上传给mes的．needMonitorLog都必须为１\r\nmes input参数变量也全部needMonitorLog必须是1';

-- Dumping data for table ptf.user_define_variable: ~41 rows (大约)
/*!40000 ALTER TABLE `user_define_variable` DISABLE KEYS */;
INSERT INTO `user_define_variable` (`variableID`, `variableName`, `variableTypeID`, `value`, `description`, `valueTypes`, `variableLength`, `plc_rw_area_did`, `plc_address`, `needMonitorLog`, `remark`, `datatime`) VALUES
	(8, 'ClearHmiSettingMonitorLog', 1, '30', '重点监控参数保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:56:24'),
	(9, 'ClearMESinterfaceLog', 1, '3', 'MES接口调用记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-17 21:57:27'),
	(10, 'ClearAlarmRecord', 1, '31', '历史报警记录保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:51:01'),
	(11, 'ClearPLCinteractiveLog', 1, '31', 'PC/PLC交互记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:51:01'),
	(12, 'dayShiftStartTime', 1, '7:30', '白班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(13, 'nightShiftStartTime', 1, '19:31', '夜班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(14, 'AssetsNO', 1, 'ACOA0022', '设备编号', 'String', 1, NULL, NULL, b'0', '不可删除此', '2019-10-19 14:20:09'),
	(27, 'Model', 1, 'M20', '生产型号', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:50'),
	(28, 'PLCversion', 2, '3.87.895', 'PLC程序版本号', 'String', 3, 21, 'D4', b'0', NULL, '2020-02-22 17:05:22'),
	(32, 'CCDVersion', 1, '3.02.6', 'CCD软件版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-09-24 19:14:34'),
	(33, 'DefaultMesUrl', 1, 'https://www.baidu.com/', '默认MES网址', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:55'),
	(46, 'ControlCode', 2, '0', 'mes控机操作的PLC地址', 'Int16', 1, 21, 'D31', b'0', NULL, '2020-08-18 13:59:02'),
	(47, 'MESstatusToPLC', 2, '10', 'MES连接状态toPLC', 'Int32', 1, 21, 'D26', b'0', NULL, '2020-08-18 13:58:30'),
	(48, 'HeatBeat', 2, '10', '心跳', 'Float', 1, 21, 'D29', b'0', NULL, '2020-08-18 13:57:55'),
	(49, 'ParentEQState', 2, '0', '设备状态', 'Int16', 1, 21, 'D20', b'0', NULL, '2020-08-18 13:57:23'),
	(50, 'AndonState', 2, '0', 'Andon状态', 'Int16', 1, 21, 'D21', b'0', NULL, '2020-08-18 13:57:26'),
	(51, 'Quantity', 2, '0', '当班产量', 'Int32', 1, 21, 'D22', b'0', NULL, '2020-08-18 13:57:31'),
	(52, 'HmiPermissionRequest', 2, '0', 'PLC获取HMI权限触发', 'Int16', 1, 21, 'D9', b'0', NULL, '2020-02-22 16:56:42'),
	(53, 'Account', 2, '0', 'PLC账户', 'String', 6, 21, 'D10', b'0', NULL, '2020-08-18 13:44:52'),
	(54, 'Code', 2, '0', 'PLC账户密码', 'String', 6, 21, 'D40', b'0', NULL, '2020-08-18 15:37:42'),
	(55, 'UserLevel', 2, '0', 'PLC账户等级', 'Int16', 1, 21, 'D28', b'0', NULL, '2020-08-18 13:57:46'),
	(65, 'localIpAdr', 1, '127.0.0.1', '本机 IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:24:52'),
	(67, 'localUdpSendPortNo', 1, '61000', '本机udp发送端口', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-01-06 13:44:43'),
	(68, 'localUdpRecvPortNo', 1, '50002', '本机udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-01-06 13:44:41'),
	(69, 'localTcpPortNo', 1, '60001', '本机tcp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-09-27 08:50:09'),
	(70, 'serverIpAdr', 1, '127.0.0.1', 'MES服务端IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:25:07'),
	(71, 'serverUdpPortNo', 1, '50000', 'MES服务端udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:27:54'),
	(72, 'HMIversion', 2, '1.25.3544', 'HMI版本', 'String', 3, 21, 'D35', b'1', NULL, '2020-08-18 15:37:55'),
	(91, 'MesReply', 2, NULL, 'MES响应状态', 'Int32', 1, 21, 'D32', b'0', NULL, '2020-08-18 13:59:07'),
	(92, 'test1', 2, '100', '预期寿命', 'Float', 1, 21, 'D1', b'0', NULL, '2020-08-18 13:44:53'),
	(94, 'ClearLog4net', 1, '30', '软件运行日志保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:45'),
	(95, 'ClearOperationLog', 1, '30', '操作日志保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:47'),
	(96, 'ClearInputDownloadHistory', 1, '180', 'Input下发历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-22 08:22:41'),
	(97, 'ClearProductionData', 1, '30', '生产记录数据保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:50'),
	(98, 'ClearInputUploadHistory', 1, '60', 'Output上载历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-22 08:37:05'),
	(99, 'A007Count', 2, '0', 'A007首件数量', 'Int16', 1, 21, 'D24', b'0', NULL, '2020-08-18 13:57:35'),
	(100, 'StateCode', 2, '0', 'A007首件信号', 'Int16', 1, 21, 'D25', b'0', NULL, '2020-08-18 13:57:37'),
	(101, 'LabChinese', 1, 'FEF', '工序名称(中文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-01-06 13:43:52'),
	(102, 'LabEnglish', 1, 'FEF', '工序名称(英文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-01-06 13:43:52'),
	(103, 'LabVersion', 1, 'FEF', '版本号', 'String', 1, NULL, NULL, b'0', NULL, '2020-08-17 21:57:27'),
	(117, 'AutoPopMonitorPage', 1, '1', '是否启用电脑无操作几分钟后自动弹出监控画面', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-17 21:57:27'),
	(167, 'DefaultAcount', 1, 'superAdmin', 'Default Acount', 'String', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:01:15');
/*!40000 ALTER TABLE `user_define_variable` ENABLE KEYS */;

-- Dumping structure for table ptf.valuetypes
CREATE TABLE IF NOT EXISTS `valuetypes` (
  `valueType` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`valueType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='变量类型';

-- Dumping data for table ptf.valuetypes: ~8 rows (大约)
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

-- Dumping structure for table ptf.variabletype
CREATE TABLE IF NOT EXISTS `variabletype` (
  `variableTypeID` int(11) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`variableTypeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='自定义变量的类型，不可更改此表的内容';

-- Dumping data for table ptf.variabletype: ~2 rows (大约)
/*!40000 ALTER TABLE `variabletype` DISABLE KEYS */;
INSERT INTO `variabletype` (`variableTypeID`, `remark`) VALUES
	(1, '系统变量(不读写PLC的变量)'),
	(2, '与PLC信号交互的变量(包括HMI监控变量)，一般为CIO、D');
/*!40000 ALTER TABLE `variabletype` ENABLE KEYS */;

-- Dumping structure for trigger ptf.TrigCalDuringTime
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `TrigCalDuringTime` BEFORE UPDATE ON `alarm_temp` FOR EACH ROW BEGIN
	set new.dispose_state = 1;
	set new.duration = TIMESTAMPDIFF(SECOND,old.alarm_time, new.dispose_time)/60;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
