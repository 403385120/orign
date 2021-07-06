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
) ENGINE=InnoDB AUTO_INCREMENT=293 DEFAULT CHARSET=utf8 COMMENT='报警记录';

-- Dumping data for table ptf.alarm_record: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_record` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=233 DEFAULT CHARSET=utf8 COMMENT='dispose_state = 0 表示暂未处理的报警\r\ndispose_state = 1 表示暂已处理的报警\r\nduration表示报警时长(min)';

-- Dumping data for table ptf.alarm_temp: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_temp` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_temp` ENABLE KEYS */;

-- Dumping structure for event ptf.daily_work
DELIMITER //
CREATE DEFINER=`root`@`localhost` EVENT `daily_work` ON SCHEDULE EVERY 12 HOUR STARTS '2019-11-29 00:00:00' ON COMPLETION PRESERVE ENABLE COMMENT '每日0点执行事件' DO begin
CALL `production_analysis_daily`();
end//
DELIMITER ;

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='EquipmentID与PLC关系表\r\n对于板卡设备，plcID为Null\r\n一个Baking设备，有多个PLC，没有ChildEQ,一个PLC对应多个EquipmentID\r\nDegassing、注液机设备，只有一个PLC，多个ChildEQ\r\n其他PLC设备，都是只有一个EquipmentID，无ChildEQ，PLC数量可能多个\r\nControlCodeAddress用于A007或者A011接口MES指令控制设备，对于PLC设备，该值为寄存器的地址。参照device_controlcode表来写数字到PLC里面。\r\nParentEQStateAddress用于A019接口上传主设备状态代码。ParentEQStateAddress表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nAndonStateAddress用于A019接口上传Andon状态代码。AndonStateAddress表示寄存器地址，或者系统变量名。读取到PLC值后，参照device_status_code表来上传字符\r\nQuantityAddress用于A019接口上传当班产量。QuantityAddress表示寄存器地址，或者系统变量名。读取到PLC值后，上传。\r\nHeatBeatAddress只针对PLC设备\r\nMesReplyAddress用来写入MES超时与否的信息\r\ndevice_controlcode表里配置了：\r\nHeatBeat\r\nMESconnected\r\nMESdisconnected\r\nMesReplyNG\r\nMesReplyOK\r\ndevice_controlcode \r\n对应的PLC值的信息\r\nUserLevelAddress 里写的值206、307、408分别表示操作、维修、开发三级权限\r\nStateCodeAddress对应的StateCode等于1表示做首件，否则等于0；CountAddress对应的Count表示首件数量\r\n';

-- Dumping data for table ptf.deivce_equipmentid_plc: ~0 rows (大约)
/*!40000 ALTER TABLE `deivce_equipmentid_plc` DISABLE KEYS */;
INSERT INTO `deivce_equipmentid_plc` (`EquipmentID`, `plcID`, `A007Jason`, `ControlCodeAddress`, `ParentEQStateAddress`, `AndonStateAddress`, `QuantityAddress`, `MESdisconnectedAddress`, `HeatBeatAddress`, `MesReplyAddress`, `HmiPermissionRequestAddress`, `AccountAddress`, `CodeAddress`, `UserLevelAddress`, `CountAddress`, `ProductModeAddress`, `SpartLimitControl`, `InputLimitControl`, `ModelAddress`, `SoftVersionAddress`) VALUES
	('AXRX016F00', NULL, '{"Header":{"SessionID":"d6fdd868-0858-44d3-9263-d63664dd59ca","FunctionID":"A007","PCName":"PCName","EQCode":"AXRX016F","SoftName":"ServerSoft","RequestTime":"2019-12-20 02:44:15 910"},"RequestInfo":{"Count":"999.000","CmdInfo":{"ControlCode":"Run","StateCode":"1","StateDesc":""},"UserInfo":{"UserID":"123","UserName":"ATL","UserLevel":"1"},"ResourceInfo":{"ResourceID":"EQ00000001","ResourceShift":"M"},"SpartInfo":[],"ModelInfo":"GC-SCC-486690-010L","ParameterInfo":[{"ParamID":"1184","StandardValue":"0.130","UpperLimitValue":"1.85","LowerLimitValue":"0.13","Description":"▼阴阳极错位距离复判规格"},{"ParamID":"251","StandardValue":"0.200","UpperLimitValue":"1.70","LowerLimitValue":"0.20","Description":"▼阴阳极错位距离"},{"ParamID":"48081","StandardValue":"9.000","UpperLimitValue":"0","LowerLimitValue":"0","Description":"层数A/B"}]}}', 'ControlCode', 'ParentEQState', 'AndonState', 'Quantity', 'MESdisconnected', NULL, 'MesReply', NULL, NULL, NULL, NULL, 'A007Count', 'StateCode', NULL, NULL, 'E:\\Andon2.0', NULL);
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

-- Dumping data for table ptf.device_alert_config: ~28 rows (大约)
/*!40000 ALTER TABLE `device_alert_config` DISABLE KEYS */;
INSERT INTO `device_alert_config` (`EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `AlertLevel`, `AlertBitAddr`, `DataTime`) VALUES
	('AXRX016F00', NULL, '0001', 'WWW', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0002', '当前算法版本与MES下发的算法版本不一致', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0003', '未收到MES开机指令', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0004', '入料定位气缸伸出异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0005', '入料定位气缸收回异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0006', '机械手1气缸伸出异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0007', '机械手1气缸收回异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0008', '机械手1真空异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0009', '机械手2气缸伸出异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0010', '机械手2气缸收回异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0011', '机械手2真空异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0012', '未Master点检', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0013', 'NG料盒满', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0014', '分拣机械手气缸伸出异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0015', '分拣机械手气缸收回异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0016', '分拣机械手真空异常', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0017', '气缸复位失败', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0018', '机械手复位失败', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0019', '入料皮带驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0020', '出料皮带驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0021', '机械手1X轴驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0022', '机械手1Y轴驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0023', '机械手2X轴驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0024', '机械手2Y轴驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0025', '分拣机械手驱动器报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0026', '连锁报警，有门被打开', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0027', '堵料报警', 'A', NULL, '2020-01-05 15:01:12'),
	('AXRX016F00', NULL, '0028', '未针规点检', 'A', NULL, '2020-01-05 15:01:12');
/*!40000 ALTER TABLE `device_alert_config` ENABLE KEYS */;

-- Dumping structure for table ptf.device_childeqcode
CREATE TABLE IF NOT EXISTS `device_childeqcode` (
  `ChildEQCode` varchar(50) NOT NULL COMMENT '填写内容：工位code',
  `ChildEQStateAddress` varchar(50) DEFAULT NULL,
  `Remark` varchar(50) NOT NULL,
  PRIMARY KEY (`ChildEQCode`),
  KEY `FK_device_childeqcode_user_define_variable` (`ChildEQStateAddress`),
  CONSTRAINT `FK_device_childeqcode_user_define_variable` FOREIGN KEY (`ChildEQStateAddress`) REFERENCES `user_define_variable` (`variableName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='一个Baking设备，有多个PLC，没有ChildEQ,一个PLC对应多个EquipmentID\r\nDegassing、注液机设备，只有一个PLC，多个ChildEQ\r\n其他PLC设备，都是只有一个EquipmentID，无ChildEQ，PLC数量可能多个\r\nChildEQStateAddress可以为系统变量，或者PLC变量，但是必须处于R类型的地址段\r\nChildEQState子设备状态为：enable和disable';

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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='从A060接口接收到的Input参数和Output参数地址配置信息\r\n\r\nEnableAddr 对应的自定义变量用于表示是否启用该Input参数, 该自定义变量的值等于10表示不启用，等于0表示启用。此表可能配置了100个参数，\r\n但是A007接口可能只下发了几个。 A007下发的那几个参数对应的EnableAddr等于0，起他的等于10.\r\n\r\n当同一个参数的input参数ID和output参数ID不同时上传接收A007指令时使用input参数ID进行接收，上传A013、A017数据时使用output参数ID进行上传；\r\n当同一个参数的input参数ID和output参数ID不同时监控input参数设定值变化A045的上传指令使用output参数ID进行上传；\r\n\r\ndownloadStatus等于1表示下载完成.\r\nMES下发model后软件需要将downloadStatus字段值改为0.\r\nvariableTypeID与variableType表里的variableTypeID对应，默认为0，表示刚从MES下载此配置，暂未判断addr为PLC地址，还是系统变量名称';

-- Dumping data for table ptf.device_inputoutput_config: ~6 rows (大约)
/*!40000 ALTER TABLE `device_inputoutput_config` DISABLE KEYS */;
INSERT INTO `device_inputoutput_config` (`ID`, `EquipmentID`, `plcID`, `SendParamID`, `UploadParamID`, `ParamName`, `Type`, `SettingValueAddr`, `UpperLimitValueAddr`, `LowerLimitValueAddr`, `LimitControl`, `InputChangeMonitorAddr`, `ActualValueAddr`, `BycellOutputAddr`, `ParamValueRatio`) VALUES
	(1, 'AXRX016F00', NULL, '51011', '51011', '电压', 'Float', 'voltage', 'voltage_up', 'voltage_down', 'EnableInputFloat', 'voltage', 'voltage', 'voltage', 1),
	(2, 'AXRX016F00', NULL, '51012', '51012', '电流', 'Float', 'current', 'current_up', 'current_down', 'EnableInputFloat', 'current', 'current', 'current', 1),
	(3, 'AXRX016F00', NULL, '1184', '1184', '阴阳极错位距离复判规格', 'Float', 'RetestSpec', 'RetestSpec_up', 'RetestSpec_down', 'EnableInputFloat', 'RetestSpec', 'RetestSpec', 'RetestSpec', 1),
	(4, 'AXRX016F00', NULL, '251', '251', '阴阳极错位距离', 'Float', 'TestSpec', 'TestSpec_up', 'TestSpec_down', 'EnableInputFloat', 'TestSpec', 'TestSpec', 'TestSpec', 1),
	(5, 'AXRX016F00', NULL, '48081', '48081', '层数A/C', 'Int32', 'LayerACNum', 'LayerACNum_up', 'LayerACNum_down', 'EnableInputInt', 'LayerACNum', 'LayerACNum', 'LayerACNum', 1),
	(6, 'AXRX016F00', NULL, '52197', '52197', '层数B/D', 'Int32', 'LayerBDNum', 'LayerBDNum_up', 'LayerBDNum_down', 'EnableInputInt', 'LayerBDNum', 'LayerBDNum', 'LayerBDNum', 1);
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

-- Dumping data for table ptf.device_spart_config: ~2 rows (大约)
/*!40000 ALTER TABLE `device_spart_config` DISABLE KEYS */;
INSERT INTO `device_spart_config` (`id`, `EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `MesSettingUsedLife`, `NeedDownLoadSpartExpectedLifetimeToPLC`, `SettingValueAddr`, `SettingActualValueAddr`, `ActualValueAddr`, `LimitControl`, `ParamValueRatio`, `PercentWarning`, `ChangeDate`, `ChangeUser`) VALUES
	(1, 'AXRX016F00', NULL, 'XRAY-GG00-0', '光管', 'Float', 50, 0, 0, 'GG00_expect', 'GG00_expect', 'GG00_curr', '', 1, 0.9000000, '2019-11-19 23:53:22', 'ATL'),
	(3, 'AXRX016F00', NULL, 'XRAY-ZQQ0-0', '增强器', 'Float', 120, 0, 0, 'ZQQ0_expect', 'ZQQ0_expect', 'ZQQ0_curr', '', 1, 0.9000000, '2019-11-29 10:46:47', 'ATL');
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
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COMMENT='用户自定义变量里重要参数记录，数据有变化的时候才记录';

-- Dumping data for table ptf.hmi_setting_monitor_log: ~0 rows (大约)
/*!40000 ALTER TABLE `hmi_setting_monitor_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `hmi_setting_monitor_log` ENABLE KEYS */;

-- Dumping structure for table ptf.hmi_setting_monitor_variatype
CREATE TABLE IF NOT EXISTS `hmi_setting_monitor_variatype` (
  `variableTypeID` int(11) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`variableTypeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='重要参数监控的变量类型';

-- Dumping data for table ptf.hmi_setting_monitor_variatype: ~3 rows (大约)
/*!40000 ALTER TABLE `hmi_setting_monitor_variatype` DISABLE KEYS */;
INSERT INTO `hmi_setting_monitor_variatype` (`variableTypeID`, `remark`) VALUES
	(1, '系统变量,变量名为自定义变量'),
	(2, 'PLC地址'),
	(3, 'PLC变量,变量名为自定义PLC变量名');
/*!40000 ALTER TABLE `hmi_setting_monitor_variatype` ENABLE KEYS */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1105 DEFAULT CHARSET=utf8 COMMENT='input参数下发历史数据以及手动上传PLC的历史数据\r\nEQID为设备名称：Baking设备必填字段，因为每个炉子可能做不一样的Model,每个炉子下发的时间点不一样。\r\nDataTime表示从PLC上传的时间或者下发到PLC的时间\r\nmodel字段表示当前生产的型号\r\ninput_variableDID表示input_variable表里的主键ID\r\n则需要在user_fefine_variable表里建立20个控制温度变量，个对应1个炉子，每个控制温度变量绑定一个PLC。';

-- Dumping data for table ptf.input_variable_history: ~0 rows (大约)
/*!40000 ALTER TABLE `input_variable_history` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=1060 DEFAULT CHARSET=utf8 COMMENT='软件运行日志\r\nloglevel=INFO日志是会在在UI界面上显示查看的\r\nloglevel=DEBUG日志只是在txt格式的log文件里查看到\r\nloglevel=ERROR表示代码逻辑异常的日志\r\nloglevel=WARN表示警告提醒的日志信息\r\nloglevel=FATAl表示软件try catch到的异常信息';

-- Dumping data for table ptf.log4net: ~38 rows (大约)
/*!40000 ALTER TABLE `log4net` DISABLE KEYS */;
INSERT INTO `log4net` (`id`, `logdate`, `loglevel`, `logger`, `message`, `exception`, `softwareName`) VALUES
	(1022, '2020-08-18 17:23:17.412', 'INFO', '', '软件启动', '', 'PTF'),
	(1023, '2020-08-18 17:23:19.185', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(1024, '2020-08-18 17:23:19.249', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(1025, '2020-08-18 17:23:22.743', 'INFO', '', '系统配置错误', '', 'PTF'),
	(1026, '2020-08-18 17:23:23.141', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(1027, '2020-08-18 17:23:23.248', 'INFO', '', '08-18 17:23:23 设备ID:AXRX016F00注册成功', '', 'PTF'),
	(1028, '2020-08-18 17:25:20.366', 'INFO', '', '软件启动', '', 'PTF'),
	(1029, '2020-08-18 17:25:21.947', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(1030, '2020-08-18 17:25:22.014', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(1031, '2020-08-18 17:25:22.091', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(1032, '2020-08-18 17:25:25.910', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(1033, '2020-08-18 17:25:26.048', 'INFO', '', '08-18 17:25:26 设备ID:AXRX016F00注册成功', '', 'PTF'),
	(1034, '2020-08-18 17:25:26.076', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(1035, '2020-08-18 17:25:26.082', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(1036, '2020-08-18 17:25:26.097', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(1037, '2020-08-18 17:25:26.115', 'INFO', '', 'A019发送设备状态 AXRX016F00', '', 'PTF'),
	(1038, '2020-08-18 17:25:26.126', 'INFO', '', 'MES返回的errmsg：A022---- 00   ', '', 'PTF'),
	(1039, '2020-08-18 17:25:26.132', 'INFO', '', 'A021发送当前易损件信息给MES AXRX016F00', '', 'PTF'),
	(1040, '2020-08-18 17:25:27.426', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(1041, '2020-08-18 17:25:27.439', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(1042, '2020-09-03 09:53:13.609', 'INFO', '', '软件启动', '', 'PTF'),
	(1043, '2020-09-03 09:53:14.973', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(1044, '2020-09-03 09:53:15.848', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200901PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200901PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200820ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200901ATL.MESInterfaceClient.cs:行号 592', '', 'PTF'),
	(1045, '2020-09-03 09:53:25.602', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(1046, '2020-09-03 09:53:26.977', 'INFO', '', '切换备用服务器', '', 'PTF'),
	(1047, '2020-09-03 09:53:26.977', 'INFO', '', '尝试切换备用服务器', '', 'PTF'),
	(1048, '2020-09-03 09:53:29.377', 'INFO', '', 'MES连接和登陆失败', '', 'PTF'),
	(1049, '2020-09-03 09:53:38.000', 'INFO', '', '连接备用服务器1号失败', '', 'PTF'),
	(1050, '2020-09-03 09:53:39.328', 'INFO', '', '软件关闭', '', 'PTF'),
	(1051, '2020-09-03 09:55:02.997', 'INFO', '', 'Software startup', '', 'PTF'),
	(1052, '2020-09-03 09:55:04.389', 'Error', '', 'Failed to deserialize [lstA047RequestFromFile] ：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200901PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200901PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200820ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200901ATL.MESInterfaceClient.cs:行号 592', '', 'PTF'),
	(1053, '2020-09-03 09:55:04.468', 'INFO', '', 'The system configuration is OK, and the PC communication has been started', '', 'PTF'),
	(1054, '2020-09-03 09:55:11.809', 'INFO', '', 'MES connect and login failed', '', 'PTF'),
	(1055, '2020-09-03 09:55:15.497', 'INFO', '', 'Switch standby server', '', 'PTF'),
	(1056, '2020-09-03 09:55:15.512', 'INFO', '', 'Try to switch the standby server ', '', 'PTF'),
	(1057, '2020-09-03 09:55:27.295', 'INFO', '', 'Connecting to the standby server 1failed ', '', 'PTF'),
	(1058, '2020-09-03 09:55:37.343', 'INFO', '', 'Connecting to the standby server 2failed ', '', 'PTF'),
	(1059, '2020-09-03 10:02:33.974', 'INFO', '', 'Software shutdown', '', 'PTF');
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
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8 COMMENT='操作记录信息/参数变更记录信息\r\n比如操作了某个运行按钮，操作了停止按钮，需要记录在此表\r\n比如软件记录MES ID的值发生了变化，则也需要将MES ID变化前的值和变化后的值记录在此表';

-- Dumping data for table ptf.log_operation: ~3 rows (大约)
/*!40000 ALTER TABLE `log_operation` DISABLE KEYS */;
INSERT INTO `log_operation` (`id`, `datatime`, `username`, `Action`) VALUES
	(38, '2020-08-18 17:25:27.441', 'superAdmin', 'MES登陆'),
	(39, '2020-09-03 09:53:29.434', 'SuperAdmin', '本地登陆'),
	(40, '2020-09-03 09:55:11.866', 'SuperAdmin', 'Local login');
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='变量自动监控记录表';

-- Dumping data for table ptf.log_plc_interactive: ~0 rows (大约)
/*!40000 ALTER TABLE `log_plc_interactive` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=67 DEFAULT CHARSET=utf8 COMMENT='MES接口上传数据表，包括了上传MES的信息和MES返回的信息\r\nResoponseTime:上位机上传数据给MES后，MES返回信息的时间，单位为ms';

-- Dumping data for table ptf.log_simple_mes_interface_execution: ~0 rows (大约)
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='离线数据缓存，待上传';

-- Dumping data for table ptf.offline_mes_interface_upload_buffer_datas: ~0 rows (大约)
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` DISABLE KEYS */;
/*!40000 ALTER TABLE `offline_mes_interface_upload_buffer_datas` ENABLE KEYS */;

-- Dumping structure for table ptf.offline_production_data
CREATE TABLE IF NOT EXISTS `offline_production_data` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `logDateTime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `ProductSN` varchar(50) NOT NULL COMMENT '电池条码',
  `Data` mediumtext NOT NULL,
  `keyValue` varchar(50) DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `logDateTime` (`logDateTime`),
  KEY `ProductSN` (`ProductSN`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8 COMMENT='A013 output离线数据缓存，待上传';

-- Dumping data for table ptf.offline_production_data: ~0 rows (大约)
/*!40000 ALTER TABLE `offline_production_data` DISABLE KEYS */;
/*!40000 ALTER TABLE `offline_production_data` ENABLE KEYS */;

-- Dumping structure for table ptf.permissions
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='此表保存了要展现的界面，和导航栏目录信息';

-- Dumping data for table ptf.permissions: ~53 rows (大约)
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` (`PermissionId`, `PermissionName`, `PermissionCode`, `Uri`, `Remark`, `DisplayOrder`, `ParentId`, `Depth`) VALUES
	(0, 'DeviceOverview', 'DeviceOverview', '', 'qq', 1, NULL, 1),
	(1, 'DeviceOverview', 'DeviceOverview.ProductOverview', 'pack://application:,,,/ATL_MES;component/ATL/ProductOverviewPage.xaml', '', 1, 0, 2),
	(2, 'Monitor', 'DeviceOverview.Monitor', NULL, '', 2, 0, 2),
	(3, 'CellsDataStatistics', 'DeviceOverview.DataCapacityStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/DataCapacityStatisticsPage.xaml', '', 3, 0, 2),
	(4, 'NGStatistics', 'DeviceOverview.NGStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/NGStatisticsPage.xaml', '', 4, 0, 2),
	(5, 'AlarmStatistics', 'DeviceOverview.AlarmStatistics', 'pack://application:,,,/ATL.UI;component/DeviceOverview/AlarmStatisticsPage.xaml', '', 5, 0, 2),
	(6, 'PC-PLCrealtimeData', 'DeviceOverview.PC-PLCrealtimeData', 'pack://application:,,,/ATL.UI;component/DeviceOverview/PC-PLCrealtimeDataPage.xaml', NULL, 6, 0, 2),
	(7, 'VersionInfo', 'DeviceOverview.Version', 'pack://application:,,,/ATL.UI;component/DeviceOverview/VersionPage.xaml', '', 7, 0, 2),
	(8, 'Repair', 'DeviceOverview.Start', 'pack://application:,,,/ATL.UI;component/DeviceOverview/StartPage.xaml', '', 8, 0, 2),
	(20, 'MES', 'MES', NULL, '', 2, NULL, 1),
	(21, 'MESweb', 'MES.MESweb', 'pack://application:,,,/ATL.UI;component/MES/MesWebPage.xaml', '', 1, 20, 2),
	(22, 'InputParaDownload', 'MES.InputParaDownload', 'pack://application:,,,/ATL.UI;component/MES/InputParaDownloadPage.xaml', '', 2, 20, 2),
	(23, 'MES_InerfaceTest', 'MES.InerfaceTest', 'pack://application:,,,/ATL.UI;component/MES/InerfaceTestPage.xaml', '', 3, 20, 2),
	(40, 'Maintain', 'Maintain', NULL, '', 3, NULL, 1),
	(41, 'RealTimeAlarm', 'Maintain.RealTime', 'pack://application:,,,/ATL.UI;component/Maintain/RealTimeAlarmPage.xaml', '', 1, 40, 2),
	(42, 'HistoryAlarm', 'Maintain.History', 'pack://application:,,,/ATL.UI;component/Maintain/HistoryAlarmPage.xaml', '', 2, 40, 2),
	(43, 'SpotInspection', 'Maintain.SpotInspection', 'pack://application:,,,/ATL.UI;component/Maintain/SpotInspectionPage.xaml', NULL, 3, 40, 2),
	(44, 'VulnerableStatistic', 'Maintain.VulnerableStatistic', 'pack://application:,,,/ATL.UI;component/Maintain/VulnerableStatisticPage.xaml', NULL, 4, 40, 2),
	(80, 'SystemSetting', 'SystemSetting', NULL, '', 5, NULL, 1),
	(81, 'Menus', 'SystemSetting.Menus', 'pack://application:,,,/ATL.UI;component/SystemSetting/MenuManagementPage.xaml', '软件工程师才可看', 1, 80, 2),
	(82, 'ParameterConfig', 'SystemSetting.ParameterConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/ParameterConfigPage.xaml', NULL, 2, 80, 2),
	(83, 'PLCcommConfig', 'SystemSetting.PLCcommConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/PLCconfigPage.xaml', '系统管理员可视', 3, 80, 2),
	(84, 'UserSettingParameter', 'SystemSetting.UserSettingParameter', 'pack://application:,,,/ATL.UI;component/SystemSetting/UserSettingParameterPage.xaml', NULL, 4, 80, 2),
	(86, 'AlarmConfig', 'SystemSetting.AlarmConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/AlarmConfigPage.xaml', '', 6, 80, 2),
	(87, 'PLCPCinteractiveCfg', 'SystemSetting.PLCPCinteractive', 'pack://application:,,,/ATL.UI;component/SystemSetting/PLCPCinteractivePage.xaml', NULL, 7, 80, 2),
	(88, 'SpotInspectioCfg', 'SystemSetting.SpotInspectionConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/SpotInspectionConfigPage.xaml', NULL, 8, 80, 2),
	(89, 'InputOutputCfg', 'SystemSetting.InputConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/InputConfigPage.xaml', '', 9, 80, 2),
	(90, 'VulnerableAddressCfg', 'SystemSetting.VulnerableAddressConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/VulnerableAddressConfigPage.xaml', NULL, 10, 80, 2),
	(91, 'SystemConfig', 'SystemSetting.SystemConfig', 'pack://application:,,,/ATL.UI;component/SystemSetting/SystemConfig.xaml', NULL, 11, 80, 2),
	(100, 'LogQuery', 'LogQuery', NULL, '', 6, NULL, 1),
	(101, 'MESinterface', 'LogQuery.MESinterface', 'pack://application:,,,/ATL.UI;component/LogQuery/MESinterfacePage.xaml', NULL, 1, 100, 2),
	(102, 'PLCinteractive', 'LogQuery.PLCinteractive', 'pack://application:,,,/ATL.UI;component/LogQuery/PLCinteractivePage.xaml', NULL, 2, 100, 2),
	(103, 'HMIparachanged', 'LogQuery.HMIparachanged', 'pack://application:,,,/ATL.UI;component/LogQuery/HMIparachanged.xaml', '', 3, 100, 2),
	(104, 'HistoryInputOutput', 'LogQuery.HistoryInputOutput', 'pack://application:,,,/ATL.UI;component/LogQuery/HistoryInputOutput.xaml', '', 4, 100, 2),
	(105, 'Operation', 'LogQuery.Operation', 'pack://application:,,,/ATL.UI;component/LogQuery/OperationPage.xaml', NULL, 5, 100, 2),
	(107, 'SoftwareLogReport', 'LogQuery.SoftwareLogReport', 'pack://application:,,,/ATL.UI;component/LogQuery/SoftwareLogReport.xaml', NULL, 7, 100, 2),
	(108, 'ProductionData', 'LogQuery.ProductionData', 'pack://application:,,,/ATL.UI;component/LogQuery/ProductionData.xaml', NULL, 10, 100, 2),
	(120, 'UserManager', 'UserManager', NULL, '', 7, NULL, 1),
	(121, 'Login', 'UserManager.Login', 'pack://application:,,,/ATL.UI;component/UserManager/LoginManagerPage.xaml', '', 1, 120, 2),
	(122, 'Personal', 'UserManager.Personal', 'pack://application:,,,/ATL.UI;component/UserManager/PersonalInfoPage.xaml', NULL, 2, 120, 2),
	(123, 'User', 'UserManager.User', 'pack://application:,,,/ATL.UI;component/UserManager/UserManagerPage.xaml', NULL, 3, 120, 2),
	(124, 'Role', 'UserManager.Role', 'pack://application:,,,/ATL.UI;component/UserManager/RoleManagerPage.xaml', NULL, 4, 120, 2),
	(140, 'DeviceControl', 'DeviceControl', NULL, '', 8, NULL, 1),
	(141, '运动控制', 'DeviceControl.MotionControl', NULL, NULL, 1, 140, 2),
	(142, '光管控制', 'DeviceControl.XrayTubeControl', NULL, NULL, 2, 140, 2),
	(143, '相机设置', 'DeviceControl.CameraSetting', NULL, NULL, 3, 140, 2),
	(144, '相机标定', 'DeviceControl.CameraCalibration', NULL, NULL, 4, 140, 2),
	(145, '检测参数', 'DeviceControl.CheckParamsSettings', NULL, NULL, 5, 140, 2),
	(146, '算法测试', 'DeviceControl.InspectTest', 'pack://application:,,,/PTF;component/InspectTestPage.xaml', NULL, 6, 140, 2),
	(147, '状态监视', 'DeviceControl.DashBoard', 'pack://application:,,,/PTF;component/DashBoardPage.xaml', NULL, 7, 140, 2),
	(148, '点检设置', 'DeviceControl.TestCode', 'pack://application:,,,/PTF;component/TestCodesPage.xaml', NULL, 8, 140, 2),
	(149, '人工复判', 'DeviceControl.ManualRecheck', 'pack://application:,,,/PTF;component/ManualRecheckPage.xaml', NULL, 9, 140, 2),
	(150, 'FQA', 'DeviceControl.ManualRecheckFQA', 'pack://application:,,,/PTF;component/ManualRecheckPage.xaml', NULL, 10, 140, 2);
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
	('C', 16, 'omron'),
	('D', 16, 'melsec,keyence,Panasonic,omron'),
	('DB', 16, 'siemens'),
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
  PRIMARY KEY (`plc_ID`),
  UNIQUE KEY `deviceName` (`plcName`),
  KEY `FK_plc_config_plc_protocol` (`ProtocolName`),
  CONSTRAINT `FK_plc_config_plc_protocol` FOREIGN KEY (`ProtocolName`) REFERENCES `plc_protocol` (`ProtocolName`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='PLC通信配置表';

-- Dumping data for table ptf.plc_config: ~0 rows (大约)
/*!40000 ALTER TABLE `plc_config` DISABLE KEYS */;
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

-- Dumping data for table ptf.plc_interactive_invariables: ~0 rows (大约)
/*!40000 ALTER TABLE `plc_interactive_invariables` DISABLE KEYS */;
/*!40000 ALTER TABLE `plc_interactive_invariables` ENABLE KEYS */;

-- Dumping structure for table ptf.plc_protocol
CREATE TABLE IF NOT EXISTS `plc_protocol` (
  `ProtocolName` varchar(50) NOT NULL COMMENT '通信协议',
  `model` varchar(100) DEFAULT NULL COMMENT '系列',
  `remark` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ProtocolName`),
  KEY `ProtocolName` (`ProtocolName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC通信协议';

-- Dumping data for table ptf.plc_protocol: ~21 rows (大约)
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
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8 COMMENT='PLC读写区域设置';

-- Dumping data for table ptf.plc_rw_config: ~0 rows (大约)
/*!40000 ALTER TABLE `plc_rw_config` DISABLE KEYS */;
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

-- Dumping structure for procedure ptf.production_analysis_daily
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `production_analysis_daily`()
    COMMENT '每日生产数据分析'
BEGIN
   DECLARE ok_count INT default 0;
	DECLARE ng_count INT default 0;
   DECLARE ok_count_e INT default 0;
	DECLARE ng_count_e INT default 0;
	DECLARE model1 varchar(50) default '';
	DECLARE ngcode1 varchar(50) default '';
	declare index1 int default 0;
	declare allcount int default 0;
	declare allcount_e int default 0;
	declare yield float default 0;
	
	declare index2 int default 0;
	declare ngcode2 varchar(50) default '';
	declare count2 int default 0;
	declare count2_e int default 0;
	
	declare all_day int default 1; -- 可以修改统计的天数
	declare log_day int default 0;
	declare is_exist int default 0;
	
	declare ok_code varchar(50) default '';

	select ngcode into ok_code from ngcode where NGCode like '%OK%' limit 1;

-- 循环每天的统计
while all_day>0 do
	set log_day=log_day+1;
	select count(distinct(Model)) into index1 from production_data where to_days(now()) - to_days(ProductDate) =log_day or to_days(now()) - to_days(ProductDate) =log_day+1;
	while index1>0 do
	  prepare s1 from 'select distinct(Model) into @ga from production_data where to_days(now()) - to_days(ProductDate) =? or to_days(now()) - to_days(ProductDate) =?+1 limit ?,1';
	  SET @a=log_day;
	  SET @b=index1-1;
	  execute s1 using @a,@a,@b;
	  DEALLOCATE PREPARE s1;
	  set model1=@ga;
	  
	  -- 白班 统计OK,NG数目
	 -- select count(model) into is_exist from production_statistics where model=model1 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='M';
	  if not exists (select model from production_statistics where model=model1 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='M') then
		  select count(1) into allcount from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day and hour(ProductDate)>7 and hour(ProductDate)<20;
		  select count(1) into ok_count from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day and hour(ProductDate)>7 and hour(ProductDate)<20 and ngcode=ok_code;
		  set ng_count= allcount-ok_count;
		  if allcount<>0 then
		    set yield=if(allcount=0,0, cast(cast(ok_count as decimal(18,3))/cast(allcount as decimal(18,3)) as decimal(18,3)));
		    INSERT INTO `production_statistics` (`model`, `MCCollectDDate`,`ME`, `NGcount`, `OKcount`, `InCount`, `OKRate`) VALUES(model1,date_add(date_sub(CURDATE(),interval log_day day),interval 20 hour),'M',ng_count,ok_count,allcount,yield);
		  end if;
		end if;
	  -- 夜班 统计OK,NG数目
	 -- select count(model) into is_exist from production_statistics where model=model1 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='E';
	  if not exists (select model from production_statistics where model=model1 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='E')  then
		  select count(1) into allcount_e from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day and hour(ProductDate)>=0 and hour(ProductDate)<8;
		  select count(1) into ok_count_e from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day and hour(ProductDate)>=0 and hour(ProductDate)<8 and ngcode=ok_code;
		  select count(1) into allcount from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day+1 and hour(ProductDate)>=20 and hour(ProductDate)<24;
		  select count(1) into ok_count from production_data where Model=model1 and to_days(now()) - to_days(ProductDate) =log_day+1 and hour(ProductDate)>=20 and hour(ProductDate)<24 and ngcode=ok_code;
		  set allcount=allcount+allcount_e;
		  set ok_count=ok_count+ok_count_e;
		  set ng_count= allcount-ok_count;
		  if allcount<>0 then
		    set yield=if(allcount=0,0, cast(cast(ok_count as decimal(18,3))/cast(allcount as decimal(18,3)) as decimal(18,3)));
		    INSERT INTO `production_statistics` (`model`, `MCCollectDDate`,`ME`, `NGcount`, `OKcount`, `InCount`, `OKRate`) VALUES(model1,date_add(date_sub(CURDATE(),interval log_day day),interval 8 hour),'E',ng_count,ok_count,allcount,yield);
		  end if;
	  end if;   
	  set index1=index1-1;
	  
	  
	  -- 统计各类NG数目
	  select count(*) into index2 from ngcode where NGCode <> ok_code;
	  while index2>0 do
	    prepare s2 from 'select NGCode into @gb from ngcode where NGCode<>? limit ?,1';
	    SET @c=ok_code;
	    SET @d=index2-1;
	    execute s2 USING @c,@d;
	    set ngcode2=@gb;
		 -- 白班     
	--	 select count(model) into is_exist from production_ngcode_statistics where model=model1 and ngcode=ngcode2 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='M';
		 if not exists(select model from production_ngcode_statistics where model=model1 and ngcode=ngcode2 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='M') then
		    select count(1) into count2 from production_data where to_days(now()) - to_days(ProductDate) =log_day and Model=model1 and ngcode=ngcode2 and hour(ProductDate)>7 and hour(ProductDate)<20;
		    INSERT INTO `production_ngcode_statistics` (`model`,`MCCollectDDate`, `ME`, `NGCode`, `Count`) VALUES (model1,date_add(date_sub(CURDATE(),interval log_day day),interval 20 hour),'M', ngcode2, count2);
	    end if;
		 -- 夜班
	--	 select count(1) into is_exist from production_ngcode_statistics where model=model1 and ngcode=ngcode2 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='E';
		 if not exists(select model from production_ngcode_statistics where model=model1 and ngcode=ngcode2 and to_days(now())-to_days(MCCollectDDate)=log_day and ME='E')  then
		    select count(1) into count2 from production_data where to_days(now()) - to_days(ProductDate) =log_day and Model=model1 and ngcode=ngcode2 and hour(ProductDate)>=0 and hour(ProductDate)<8;
		    select count(Iden) into count2_e from production_data where to_days(now()) - to_days(ProductDate) =log_day+1 and Model=model1 and ngcode=ngcode2 and hour(ProductDate)>=20 and hour(ProductDate)<24;
		    set count2=count2+count2_e;
		    INSERT INTO `production_ngcode_statistics` (`model`,`MCCollectDDate`, `ME`, `NGCode`, `Count`) VALUES (model1,date_add(date_sub(CURDATE(),interval log_day day),interval 8 hour),'E', ngcode2, count2);
		 end if;
		 set index2=index2-1;
	    end while;
	end while;
	set all_day=all_day-1;
end while;

end//
DELIMITER ;

-- Dumping structure for table ptf.production_data
CREATE TABLE IF NOT EXISTS `production_data` (
  `Iden` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `ProductSN` varchar(50) DEFAULT NULL COMMENT '电池条码',
  `Model` varchar(50) DEFAULT NULL COMMENT '生产型号',
  `quality` int(11) NOT NULL DEFAULT 1,
  `NgCode` varchar(50) DEFAULT NULL,
  `NGreason` varchar(255) DEFAULT NULL COMMENT '不良原因',
  `ProductDate` datetime NOT NULL DEFAULT current_timestamp(),
  `OutTime` datetime NOT NULL DEFAULT current_timestamp(),
  `Operator` varchar(50) DEFAULT NULL COMMENT '员工号',
  `ResultPath` varchar(128) DEFAULT NULL COMMENT '结果图Path',
  `MesImagePath` varchar(128) DEFAULT NULL COMMENT 'Mes文件路径',
  `ResourceShift` varchar(50) DEFAULT NULL COMMENT '班次',
  `A1_PhotographResult` int(8) DEFAULT NULL COMMENT 'A角拍照结果',
  `A1_OriginalImagePath` varchar(128) DEFAULT NULL COMMENT 'A角原始图Path',
  `A1_ResultImagePath` varchar(128) DEFAULT NULL COMMENT 'A角结果图Path',
  `A1_Min` float DEFAULT NULL COMMENT 'A角最小',
  `A1_Max` float DEFAULT NULL COMMENT 'A角最大',
  `A1_Distance1` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_1',
  `A1_Distance2` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_2',
  `A1_Distance3` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_3',
  `A1_Distance4` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_4',
  `A1_Distance5` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_5',
  `A1_Distance6` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_6',
  `A1_Distance7` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_7',
  `A1_Distance8` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_8',
  `A1_Distance9` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_9',
  `A1_Distance10` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_10',
  `A1_Distance11` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_11',
  `A1_Distance12` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_12',
  `A1_Distance13` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_13',
  `A1_Distance14` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_14',
  `A1_Distance15` float DEFAULT NULL COMMENT 'A角阴阳极错位距离_15',
  `A1_Angle1` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_1',
  `A1_Angle2` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_2',
  `A1_Angle3` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_3',
  `A1_Angle4` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_4',
  `A1_Angle5` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_5',
  `A1_Angle6` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_6',
  `A1_Angle7` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_7',
  `A1_Angle8` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_8',
  `A1_Angle9` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_9',
  `A1_Angle10` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_10',
  `A1_Angle11` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_11',
  `A1_Angle12` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_12',
  `A1_Angle13` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_13',
  `A1_Angle14` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_14',
  `A1_Angle15` float DEFAULT NULL COMMENT 'A角阳极轮廓与阴极的角度_15',
  `A2_PhotographResult` int(8) DEFAULT NULL COMMENT 'B角拍照结果',
  `A2_OriginalImagePath` varchar(128) DEFAULT NULL COMMENT 'B角原始图Path',
  `A2_ResultImagePath` varchar(128) DEFAULT NULL COMMENT 'B角结果图Path',
  `A2_Min` float DEFAULT NULL COMMENT 'B角最小',
  `A2_Max` float DEFAULT NULL COMMENT 'B角最大',
  `A2_Distance1` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_1',
  `A2_Distance2` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_2',
  `A2_Distance3` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_3',
  `A2_Distance4` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_4',
  `A2_Distance5` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_5',
  `A2_Distance6` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_6',
  `A2_Distance7` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_7',
  `A2_Distance8` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_8',
  `A2_Distance9` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_9',
  `A2_Distance10` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_10',
  `A2_Distance11` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_11',
  `A2_Distance12` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_12',
  `A2_Distance13` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_13',
  `A2_Distance14` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_14',
  `A2_Distance15` float DEFAULT NULL COMMENT 'B角阴阳极错位距离_15',
  `A2_Angle1` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_1',
  `A2_Angle2` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_2',
  `A2_Angle3` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_3',
  `A2_Angle4` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_4',
  `A2_Angle5` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_5',
  `A2_Angle6` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_6',
  `A2_Angle7` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_7',
  `A2_Angle8` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_8',
  `A2_Angle9` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_9',
  `A2_Angle10` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_10',
  `A2_Angle11` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_11',
  `A2_Angle12` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_12',
  `A2_Angle13` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_13',
  `A2_Angle14` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_14',
  `A2_Angle15` float DEFAULT NULL COMMENT 'B角阳极轮廓与阴极的角度_15',
  `A3_PhotographResult` int(8) DEFAULT NULL COMMENT 'C角拍照结果',
  `A3_OriginalImagePath` varchar(128) DEFAULT NULL COMMENT 'C角原始图Path',
  `A3_ResultImagePath` varchar(128) DEFAULT NULL COMMENT 'C角结果图Path',
  `A3_Min` float DEFAULT NULL COMMENT 'C角最小',
  `A3_Max` float DEFAULT NULL COMMENT 'C角最大',
  `A3_Distance1` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_1',
  `A3_Distance2` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_2',
  `A3_Distance3` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_3',
  `A3_Distance4` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_4',
  `A3_Distance5` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_5',
  `A3_Distance6` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_6',
  `A3_Distance7` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_7',
  `A3_Distance8` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_8',
  `A3_Distance9` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_9',
  `A3_Distance10` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_10',
  `A3_Distance11` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_11',
  `A3_Distance12` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_12',
  `A3_Distance13` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_13',
  `A3_Distance14` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_14',
  `A3_Distance15` float DEFAULT NULL COMMENT 'C角阴阳极错位距离_15',
  `A3_Angle1` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_1',
  `A3_Angle2` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_2',
  `A3_Angle3` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_3',
  `A3_Angle4` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_4',
  `A3_Angle5` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_5',
  `A3_Angle6` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_6',
  `A3_Angle7` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_7',
  `A3_Angle8` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_8',
  `A3_Angle9` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_9',
  `A3_Angle10` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_10',
  `A3_Angle11` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_11',
  `A3_Angle12` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_12',
  `A3_Angle13` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_13',
  `A3_Angle14` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_14',
  `A3_Angle15` float DEFAULT NULL COMMENT 'C角阳极轮廓与阴极的角度_15',
  `A4_PhotographResult` int(8) DEFAULT NULL COMMENT 'D角拍照结果',
  `A4_OriginalImagePath` varchar(128) DEFAULT NULL COMMENT 'D角原始图Path',
  `A4_ResultImagePath` varchar(128) DEFAULT NULL COMMENT 'D角结果图Path',
  `A4_Min` float DEFAULT NULL COMMENT 'D角最小',
  `A4_Max` float DEFAULT NULL COMMENT 'D角最大',
  `A4_Distance1` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_1',
  `A4_Distance2` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_2',
  `A4_Distance3` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_3',
  `A4_Distance4` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_4',
  `A4_Distance5` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_5',
  `A4_Distance6` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_6',
  `A4_Distance7` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_7',
  `A4_Distance8` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_8',
  `A4_Distance9` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_9',
  `A4_Distance10` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_10',
  `A4_Distance11` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_11',
  `A4_Distance12` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_12',
  `A4_Distance13` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_13',
  `A4_Distance14` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_14',
  `A4_Distance15` float DEFAULT NULL COMMENT 'D角阴阳极错位距离_15',
  `A4_Angle1` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_1',
  `A4_Angle2` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_2',
  `A4_Angle3` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_3',
  `A4_Angle4` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_4',
  `A4_Angle5` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_5',
  `A4_Angle6` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_6',
  `A4_Angle7` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_7',
  `A4_Angle8` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_8',
  `A4_Angle9` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_9',
  `A4_Angle10` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_10',
  `A4_Angle11` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_11',
  `A4_Angle12` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_12',
  `A4_Angle13` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_13',
  `A4_Angle14` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_14',
  `A4_Angle15` float DEFAULT NULL COMMENT 'D角阳极轮廓与阴极的角度_15',
  `RecheckState` int(4) DEFAULT 0,
  `RecheckTime` datetime DEFAULT NULL COMMENT '手动检测时间',
  `RecheckUserID` varchar(20) DEFAULT NULL COMMENT '重判用户ID',
  `FQATime` datetime DEFAULT NULL COMMENT 'FQA检测时间',
  `FQAUser` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Iden`),
  KEY `index_batteryBarCode` (`ProductSN`),
  KEY `Model` (`Model`),
  KEY `FK_production_data_ngcode` (`NgCode`),
  KEY `ProductDate` (`ProductDate`),
  CONSTRAINT `FK_production_data_ngcode` FOREIGN KEY (`NgCode`) REFERENCES `ngcode` (`NGCode`)
) ENGINE=InnoDB AUTO_INCREMENT=186 DEFAULT CHARSET=utf8 COMMENT='每个工艺的设备的电池数据会不一致，需要各个厂家自己设计此表\r\n必须要有条码字段ProductSN、时间字段OutTime、型号字段Model、NgCode字段\r\n\r\nRecheckState 手动检测结果：\r\n0：未检测\r\n1：重判OK\r\n2：重判NG\r\n3： FQA判定NG\r\n4：FQA判定OK\r\n5：已上传FQAOK的结果\r\n6：已上传复判NG的结果\r\n9：复检待判';

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='易损件下线记录';

-- Dumping data for table ptf.quickwearparthistoryinfo: ~0 rows (大约)
/*!40000 ALTER TABLE `quickwearparthistoryinfo` DISABLE KEYS */;
/*!40000 ALTER TABLE `quickwearparthistoryinfo` ENABLE KEYS */;

-- Dumping structure for table ptf.recheck_production_data
CREATE TABLE IF NOT EXISTS `recheck_production_data` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `logDateTime` datetime(3) NOT NULL DEFAULT current_timestamp(3),
  `ProductSN` varchar(50) NOT NULL COMMENT '电池条码',
  `Data` mediumtext NOT NULL,
  `keyValue` varchar(50) DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `ProductSN` (`ProductSN`),
  KEY `logDateTime` (`logDateTime`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COMMENT='A013 output NG数据缓存，待人工复检后上传';

-- Dumping data for table ptf.recheck_production_data: ~9 rows (大约)
/*!40000 ALTER TABLE `recheck_production_data` DISABLE KEYS */;
INSERT INTO `recheck_production_data` (`id`, `logDateTime`, `ProductSN`, `Data`, `keyValue`) VALUES
	(3, '2019-12-28 17:07:46.849', 'W76951CP14A5', '{"Header":{"SessionID":"f35d6889-4bd9-42ea-b1db-c2a0fe31d400","FunctionID":"A013","PCName":"NPRD-NFNI7062-P","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-28 17:07:46 633"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP14A5","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.54","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.518","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.474","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.441","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.441","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.386","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.298","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.111","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.418","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.495","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.506","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.572","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.572","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.66","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.693","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5401","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5401","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191228170745989_W76951CP14A5_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '1.2'),
	(4, '2019-12-31 12:20:17.625', 'W76951CP19CE', '{"Header":{"SessionID":"74e09d7f-5dd6-42d6-b73c-33879f7f9239","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:20:17 615"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP19CE","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8231106","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9123635","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9222805","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9222805","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9421145","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9817825","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9916995","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.02145","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.5851027","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6644387","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.4561818","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7834426","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9024465","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9024465","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7239406","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122017473_W76951CP19CE_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(5, '2019-12-31 12:20:32.460', 'W76951AP097B', '{"Header":{"SessionID":"efb26a31-e030-4a60-aa6e-3a7aca54f0ba","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:20:32 458"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951AP097B","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9520315","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.299126","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.398296","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.477632","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.487549","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.437964","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.338794","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.011533","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.4165138","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.2776759","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.2380079","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.1884229","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.2380079","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.3570118","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.5057667","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122032340_W76951AP097B_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(6, '2019-12-31 12:20:34.676', 'W76951DP13C0', '{"Header":{"SessionID":"aa256dff-f10c-41d6-b96a-cfe62378bb30","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:20:34 675"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951DP13C0","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.451","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.363","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.308","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.264","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.143","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.187","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.264","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.572","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.507","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.573","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.694","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.782","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.782","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.749","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.562","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122034551_W76951DP13C0_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(7, '2019-12-31 12:21:00.941', 'W76951CP19D7', '{"Header":{"SessionID":"6767539c-00be-4d30-adae-eec05206c2d0","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:21:00 940"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP19D7","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.5454347","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7239406","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8231106","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8131936","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8032766","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6941896","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.5851027","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.011533","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7735256","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8330275","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8231106","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8131936","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8330275","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8528615","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9520315","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122100828_W76951CP19D7_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(8, '2019-12-31 12:21:03.327', 'W76951CP19D5', '{"Header":{"SessionID":"94b04db5-8719-4ea0-8c3f-a0563396699a","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:21:03 326"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP19D5","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.44","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.891","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.122","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.353","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.254","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.276","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.243","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.111","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.627","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.649","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.671","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.726","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.715","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.748","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.814","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122103203_W76951CP19D5_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(9, '2019-12-31 12:21:07.175', 'W76951CP19D4', '{"Header":{"SessionID":"e97ec62e-35e6-41a4-bbeb-86dc7d82b8a8","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:21:07 173"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP19D4","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.1785059","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8429446","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8032766","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8528615","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8032766","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8925295","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8627785","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7041066","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6743556","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7239406","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7437746","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7338576","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7536916","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7834426","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8330275","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122107018_W76951CP19D4_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(10, '2019-12-31 12:21:26.198', 'W76950CP06C3', '{"Header":{"SessionID":"de70e025-55ef-43e3-988a-944eca4f21ff","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:21:26 197"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76950CP06C3","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8826125","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8726956","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9321975","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9421145","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9421145","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.001616","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.001616","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.041284","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6941896","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7239406","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7437746","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7140236","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7536916","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6842726","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.6346877","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122126060_W76950CP06C3_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95'),
	(11, '2019-12-31 12:21:33.790', 'W76951CP19C9', '{"Header":{"SessionID":"d191b18d-8ffb-4ea5-802b-66da440e6d6e","FunctionID":"A013","PCName":"WPRD-I60973-O","EQCode":"AXRX016F","SoftName":"ClientSoft1","RequestTime":"2019-12-31 12:21:33 786"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"W76951CP19C9","Pass":"OK","ChildEquCode":null,"Station":"1","OutputParam":[{"ParamID":"52136","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8528615","Result":null},{"ParamID":"52137","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8032766","Result":null},{"ParamID":"52138","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7834426","Result":null},{"ParamID":"52139","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7933596","Result":null},{"ParamID":"52140","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8330275","Result":null},{"ParamID":"52141","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9123635","Result":null},{"ParamID":"52142","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9321975","Result":null},{"ParamID":"52143","SpecParamID":null,"ParamDesc":null,"ParamValue":"1.02145","Result":null},{"ParamID":"52151","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.2677588","Result":null},{"ParamID":"52152","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.4859327","Result":null},{"ParamID":"52153","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7239406","Result":null},{"ParamID":"52154","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8032766","Result":null},{"ParamID":"52155","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.8330275","Result":null},{"ParamID":"52156","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.9024465","Result":null},{"ParamID":"52157","SpecParamID":null,"ParamDesc":null,"ParamValue":"0.7735256","Result":null},{"ParamID":"51740","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51741","SpecParamID":null,"ParamDesc":null,"ParamValue":"5448","Result":null},{"ParamID":"51011","SpecParamID":null,"ParamDesc":null,"ParamValue":"59","Result":null},{"ParamID":"51012","SpecParamID":null,"ParamDesc":null,"ParamValue":"89","Result":null},{"ParamID":"51856","SpecParamID":null,"ParamDesc":null,"ParamValue":"NG_20191231122133670_W76951CP19C9_5_test.jpg","Result":null},{"ParamID":"251","SpecParamID":null,"ParamDesc":null,"ParamValue":"99.99","Result":null}]}]}}', '0.95');
/*!40000 ALTER TABLE `recheck_production_data` ENABLE KEYS */;

-- Dumping structure for table ptf.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `roleId` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `roleName` varchar(50) NOT NULL COMMENT '角色名称',
  `MesUserLevel` varchar(50) NOT NULL COMMENT 'MES里的权限内容',
  `UserLevelPLCValue` int(11) NOT NULL DEFAULT 0 COMMENT '206、307、408分别表示操作、维修、开发三级权限',
  `permissionCodes` varchar(10000) DEFAULT NULL COMMENT '菜单权限编码',
  `createTime` datetime NOT NULL DEFAULT current_timestamp() COMMENT '创建时间',
  `modifyTime` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `remark` varchar(250) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`roleId`),
  UNIQUE KEY `roleName` (`roleName`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.roles: ~5 rows (大约)
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` (`roleId`, `roleName`, `MesUserLevel`, `UserLevelPLCValue`, `permissionCodes`, `createTime`, `modifyTime`, `remark`) VALUES
	(0, 'Development Authority', 'Administrator', 408, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:23:18', '超级管理员,禁止删除'),
	(1, 'Maintain Authority', 'Maintainer', 307, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:23:18', '系统管理员'),
	(2, 'Operator Authority', 'Operator', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,UserManager,UserManager.Login,DeviceControl,DeviceControl.MotionControl,DeviceControl.XrayTubeControl,DeviceControl.CameraSetting,DeviceControl.CameraCalibration,DeviceControl.CheckParamsSettings,DeviceControl.InspectTest,DeviceControl.DashBoard,DeviceControl.TestCode,DeviceControl.ManualRecheck,DeviceControl.ManualRecheckFQA', '2019-07-09 09:50:00', '2020-08-18 17:23:18', 'ME人员'),
	(10, 'Guest Authority', 'Guest', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,MES,MES.MESweb,Maintain,DataWareHouse,SystemSetting,LogQuery,UserManager,UserManager.Login,DeviceControl,DeviceControl.DashBoard,DeviceControl.InspectTest,DeviceControl.CameraSetting,DeviceControl.XrayTubeControl,DeviceControl.MotionControl', '2019-07-02 11:04:39', '2020-08-18 17:23:18', '尚未登陆'),
	(12, 'newRole', 'new', 0, 'MES,MES.MESweb,MES.InputParaDownload,MES.InerfaceTest,UserManager,UserManager.Login', '2020-01-05 14:57:44', '2020-01-05 14:57:44', 'w');
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
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.server_backpara: ~2 rows (大约)
/*!40000 ALTER TABLE `server_backpara` DISABLE KEYS */;
INSERT INTO `server_backpara` (`ID`, `backupServerIpAdr`, `backupServerUdpPortNo`, `backupLocalUdpSendPortNo`, `backupLocalUdpRecvPortNo`, `backupLocalTcpPortNo`) VALUES
	(1, '127.0.0.1', '50000', '61001', '61000', '60001'),
	(2, '172.23.11.85', '50031', '50002', '50035', '60011');
/*!40000 ALTER TABLE `server_backpara` ENABLE KEYS */;

-- Dumping structure for table ptf.test_code
CREATE TABLE IF NOT EXISTS `test_code` (
  `RecordID` int(11) NOT NULL AUTO_INCREMENT,
  `BarCode` varchar(45) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `CreateBy` varchar(45) DEFAULT NULL,
  `Remarks` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`RecordID`),
  UNIQUE KEY `BarCode_UNIQUE` (`BarCode`),
  KEY `test_code.BarCode` (`BarCode`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8 COMMENT='点检条码表';

-- Dumping data for table ptf.test_code: ~3 rows (大约)
/*!40000 ALTER TABLE `test_code` DISABLE KEYS */;
INSERT INTO `test_code` (`RecordID`, `BarCode`, `CreateTime`, `CreateBy`, `Remarks`) VALUES
	(26, 'W76927D701DE', '2019-12-10 14:25:59', 'tttttttt', ''),
	(27, 'W76927D70047', '2019-12-10 14:25:59', 'tttttttt', ''),
	(28, 'W76927D701E2', '2019-12-10 14:25:59', 'tttttttt', '');
/*!40000 ALTER TABLE `test_code` ENABLE KEYS */;

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
	(2, 'Guest', 'Guest', 'Guest', 10, '2017-03-23 21:05:21', 1, '2020-09-03 09:55:03', NULL),
	(3, 'chen', 'chen', 'Michal', 12, '0001-01-01 00:00:00', 0, '2020-01-05 14:58:59', NULL),
	(4, 'SuperAdmin', 'SuperAdmin', 'SuperAdmin', 0, '2019-07-08 13:22:06', 0, '2020-09-03 09:55:11', NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=612 DEFAULT CHARSET=utf8 COMMENT='用户定义变量表。variableTypeID等于1表示为系统变量，系统变量与PLC变量，不是在同一个UI界面上显示的。\r\n系统变量不会与PLC地址绑定。非系统变量可能会与PLC地址绑定关联，用于接口数据上传。\r\n每个MES ID的变量关联的plc_rw_area_did都必须是Ｒ读类型的．MES ID的变量属于OUTPUT参数，上传给mes的．needMonitorLog都必须为１\r\nmes input参数变量也全部needMonitorLog必须是1';

-- Dumping data for table ptf.user_define_variable: ~250 rows (大约)
/*!40000 ALTER TABLE `user_define_variable` DISABLE KEYS */;
INSERT INTO `user_define_variable` (`variableID`, `variableName`, `variableTypeID`, `value`, `description`, `valueTypes`, `variableLength`, `plc_rw_area_did`, `plc_address`, `needMonitorLog`, `remark`, `datatime`) VALUES
	(8, 'ClearHmiSettingMonitorLog', 1, '30', 'Clear Hmi Setting MonitorLog(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(9, 'ClearMESinterfaceLog', 1, '3', 'Clear MES interface Log(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(10, 'ClearAlarmRecord', 1, '31', 'Clear Alarm Record(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(11, 'ClearPLCinteractiveLog', 1, '31', 'Clear PLC interactive Log(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(12, 'dayShiftStartTime', 1, '7:30', 'Day Shift StartTime', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(13, 'nightShiftStartTime', 1, '19:31', 'Night Shift StartTime', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(14, 'AssetsNO', 1, 'AXRX016F00', '设备编号', 'String', 1, NULL, NULL, b'0', '不可删除此', '2020-01-05 15:00:44'),
	(27, 'Model', 1, 'M20', 'Model', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(28, 'PLCversion', 1, '3.87.895', 'PLC Version', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(31, 'SoftVersion', 1, 'V01.02.01.191028', 'ATL_MES SoftVersion', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(32, 'CCDVersion', 1, '3.02.6', 'CCD Version', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(33, 'DefaultMesUrl', 1, 'https://www.baidu.com/', 'DefaultMesUrl', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(46, 'ControlCode', 1, '1', 'MES ControlCode', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(47, 'RunStatus', 1, '11', '设备状态', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:27:06'),
	(48, 'ProductCount', 1, '10', '产量', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:38:49'),
	(49, 'PPM', 1, '12', '设备当前PPM', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:39:26'),
	(50, 'ANDONstatus', 1, '0', '当前ANDON状态', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:40:41'),
	(51, 'CarreerScan', 1, '1', '弹夹扫码', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:41:33'),
	(52, 'CellScan', 1, '1', '电芯扫码', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:42:05'),
	(53, 'CellLoad', 1, '1', '电芯装载', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:42:35'),
	(54, 'HeatBeat', 1, '1', 'Heat Beat', 'Int32', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(55, 'MESstatusToPLC', 1, '1', 'MES Status To PLC', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(56, 'CarreerScanOK', 1, '1', '弹夹扫码成功', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:44:03'),
	(57, 'CarreerScanNG', 1, '1', '弹夹扫码失败', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:44:29'),
	(58, 'CellScanOK', 1, '1', '电芯扫码成功', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:44:59'),
	(59, 'CellScanNG', 1, '1', '电芯扫码失败', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:45:22'),
	(60, 'CellLoadOK', 1, '1', '装载成功', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:45:49'),
	(61, 'CurrCarrierBarcode', 1, 'W1234', '当前弹夹条码', 'String', 20, NULL, NULL, b'0', NULL, '2019-09-19 22:51:15'),
	(62, 'UploadToMes', 1, '1', '开始绑定上传给MES', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:47:20'),
	(63, 'UploadToMesOK', 1, '1', '绑定上传MES成功', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:47:46'),
	(64, 'UploadToMesNG', 1, '1', '绑定上传MES失败', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:48:22'),
	(65, 'localIpAdr', 1, '127.0.0.1', 'local Ip Adr', 'String', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(67, 'localUdpSendPortNo', 1, '50003', 'local Udp Send PortNo', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(68, 'localUdpRecvPortNo', 1, '50002', 'local Udp RecvPort No', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(69, 'localTcpPortNo', 1, '60001', 'local Tcp PortNo', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(70, 'serverIpAdr', 1, '127.0.0.1', 'Server IpAdr', 'String', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(71, 'serverUdpPortNo', 1, '50000', 'ServerUdpPortNo', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(72, 'HMIversion', 1, '1.25.3544', 'HMI Version', 'String', 1, NULL, NULL, b'1', NULL, '2020-09-03 09:55:03'),
	(77, 'PLCreset', 1, '1', 'PLC触发复位', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:48:44'),
	(78, 'PLCresetOK', 1, '1', '上位机准备OK', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:49:08'),
	(79, 'VolMax', 1, '5', '电压上限', 'Float', 1, NULL, NULL, b'1', NULL, '2019-10-22 16:32:48'),
	(80, 'VolMin', 1, '4', '电压下限', 'Float', 1, NULL, NULL, b'1', NULL, '2019-10-22 16:32:52'),
	(82, 'VolSetting', 1, '4.5', '电压设定值', 'Float', 1, NULL, NULL, b'1', NULL, '2019-10-22 16:32:56'),
	(90, 'MESdisconnected', 1, '0', 'MES连接状态', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-13 01:26:15'),
	(91, 'MesReply', 1, '0', 'Mes Reply State', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(93, 'ChildEQState1', 1, '0', '子设备1状态', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-05 20:17:38'),
	(94, 'ChildEQState2', 1, '0', '子设备2状态', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-05 20:17:38'),
	(95, 'ChildEQState3', 1, '0', '子设备3状态', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-05 20:17:38'),
	(96, 'ChildEQState4', 1, '0', '子设备4状态', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-05 20:17:38'),
	(97, 'HmiPermissionRequest', 1, '1', 'Hmi Permission Request', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(98, 'Account', 1, '1', 'Account', 'String', 6, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(99, 'Code', 1, '1', 'Code', 'String', 6, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(100, 'UserLevel', 1, '1', 'User Level', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(101, 'ClearLog4net', 1, '30', 'Clear RunLog(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(102, 'ClearOperationLog', 1, '30', 'Clear Operation Log(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(103, 'ClearInputDownloadHistory', 1, '30', 'Clear Input Download History(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(104, 'ClearProductionData', 1, '30', 'Clear Production Data(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(105, 'ClearInputUploadHistory', 1, '30', 'Clear Input Upload History(Day)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(106, 'A007Count', 1, '5', 'A007 First Piece Count', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(107, 'StateCode', 1, '0', 'StateCode', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(200, 'm_axisX1', 1, '1', '机械手1X轴轴号', 'Int16', 1, NULL, NULL, b'1', NULL, '2019-10-30 23:25:37'),
	(201, 'm_axisX1OutMode', 1, '4', '机械手1X轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:15'),
	(202, 'm_axisX1DirP', 1, '1', '机械手1X轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:06:27'),
	(203, 'm_axisX1DirN', 1, '0', '机械手1X轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:28'),
	(204, 'm_axisY1', 1, '2', '机械手1Y轴轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:31'),
	(205, 'm_axisY1OutMode', 1, '4', '机械手1Y轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:34'),
	(206, 'm_axisY1DirP', 1, '1', '机械手1Y轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:17:53'),
	(207, 'm_axisY1DirN', 1, '0', '机械手1Y轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:47'),
	(208, 'm_axisX2', 1, '0', '机械手2X轴轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:50'),
	(209, 'm_axisX2OutMode', 1, '4', '机械手2X轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:07:54'),
	(210, 'm_axisX2DirP', 1, '1', '机械手2X轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:22:56'),
	(211, 'm_axisX2DirN', 1, '0', '机械手2X轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:03'),
	(212, 'm_axisY2', 1, '3', '机械手2Y轴轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:06'),
	(213, 'm_axisY2OutMode', 1, '4', '机械手2Y轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:09'),
	(214, 'm_axisY2DirP', 1, '1', '机械手2Y轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:26:38'),
	(215, 'm_axisY2DirN', 1, '0', '机械手2Y轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:18'),
	(216, 'm_axisRay1', 1, '4', '光管轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:21'),
	(217, 'm_axisRay1OutMode', 1, '4', '光管轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:24'),
	(218, 'm_axisRay1DirP', 1, '0', '光管轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:44'),
	(219, 'm_axisRay1DirN', 1, '1', '光管轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:30:46'),
	(220, 'm_axisNg', 1, '10', '分拣机械手轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:47'),
	(221, 'm_axisNgOutMode', 1, '4', '分拣机械手轴输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:08:53'),
	(222, 'm_axisNgDirP', 1, '1', '分拣机械手轴运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:03:37'),
	(223, 'm_axisNgDirN', 1, '0', '分拣机械手轴运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:03:40'),
	(224, 'm_axisIn', 1, '6', '入料皮带轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:09:08'),
	(225, 'm_axisInOutMode', 1, '4', '入料皮带输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:09:14'),
	(226, 'm_axisInDirP', 1, '0', '入料皮带运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:09:19'),
	(227, 'm_axisInDirN', 1, '1', '入料皮带运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:38:24'),
	(228, 'm_axisOut', 1, '7', '出料皮带轴号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:09:27'),
	(229, 'm_axisOutOutMode', 1, '4', '出料皮带输出模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:09:34'),
	(230, 'm_axisOutDirP', 1, '0', '出料皮带运动正方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:10:00'),
	(231, 'm_axisOutDirN', 1, '1', '出料皮带运动负方向', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 20:41:09'),
	(232, 'm_limitX1Min', 1, '-13000000', '机械手1X轴负限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:43:52'),
	(233, 'm_limitX1Max', 1, '0', '机械手1X轴正限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:43:58'),
	(234, 'm_limitY1Min', 1, '-8000000', '机械手1Y轴负限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:44:56'),
	(235, 'm_limitY1Max', 1, '0', '机械手1Y轴正限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:44:08'),
	(236, 'm_limitX2Min', 1, '-13000000', '机械手2X轴负限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:45:04'),
	(237, 'm_limitX2Max', 1, '0', '机械手2X轴正限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:44:17'),
	(238, 'm_limitY2Min', 1, '-8000000', '机械手2Y轴负限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:45:08'),
	(239, 'm_limitY2Max', 1, '0', '机械手2Y轴正限位', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:44:30'),
	(240, 'm_dInSpeedTemp', 1, '400', '入料皮带速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:13:49'),
	(241, 'm_dOutSpeedTemp', 1, '400', '出料皮带速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-24 10:41:01'),
	(242, 'm_dNgSpeedTemp', 1, '1000', '分拣机械手速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:04:11'),
	(243, 'm_dToCatchSpeed2Temp', 1, '1300', '机械手2到取料点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:06'),
	(244, 'm_dTo1stInspectSpeed2Temp', 1, '1000', '机械手2到AC点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:13'),
	(245, 'm_dTo2ndInspectSpeed2Temp', 1, '1000', '机械手2到BD点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:26'),
	(246, 'm_dToPutSpeed2Temp', 1, '1300', '机械手2到放料点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:31'),
	(247, 'm_dToBoardsideSpeed2Temp', 1, '1300', '机械手2到收臂点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:37'),
	(248, 'm_dToWaitSpeed2Temp', 1, '1300', '机械手2到待命点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-24 11:40:41'),
	(249, 'm_dToCatchSpeed1Temp', 1, '800', '机械手1到取料点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:04:52'),
	(250, 'm_dTo1stInspectSpeed1Temp', 1, '600', '机械手1到AC点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:04:56'),
	(251, 'm_dTo2ndInspectSpeed1Temp', 1, '600', '机械手1到BD点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-15 10:25:56'),
	(252, 'm_dToPutSpeed1Temp', 1, '800', '机械手1到放料点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:02'),
	(253, 'm_dToBoardsideSpeed1Temp', 1, '800', '机械手1到收臂点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:06'),
	(254, 'm_dToWaitSpeed1Temp', 1, '800', '机械手1到待命点速度', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:10'),
	(255, 'm_outInPush', 1, '27', '入料定位输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-04 15:59:49'),
	(256, 'm_inZ2Down', 1, '4', '机械手2气缸下输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:17:11'),
	(257, 'm_inZ2Up', 1, '2', '机械手2气缸上输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:17:29'),
	(258, 'm_inZ1Down', 1, '8', '机械手1气缸下输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:17:36'),
	(259, 'm_inZ1Up', 1, '6', '机械手1气缸上输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:18:13'),
	(260, 'm_inInBack', 1, '19', '入料定位伸出输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:18'),
	(261, 'm_inInPush', 1, '20', '入料定位收回输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:21'),
	(262, 'm_inZ3Down', 1, '10', '分拣机械手气缸下输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:18:53'),
	(263, 'm_inZ3Up', 1, '9', '分拣机械手气缸上输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:09'),
	(264, 'm_inBoxPush', 1, '14', 'NG料盒气缸伸出输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:16'),
	(265, 'm_inBoxBack', 1, '13', 'NG料盒气缸收回输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:35'),
	(266, 'm_inInAlarm', 1, '19', '入料皮带报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:39'),
	(267, 'm_inInAlarmOn', 1, '0', '入料皮带报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:44'),
	(268, 'm_inOutAlarm', 1, '20', '出料皮带报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:19:47'),
	(269, 'm_inOutAlarmOn', 1, '0', '出料皮带报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:20:36'),
	(270, 'm_inX1Alarm', 1, '2', '机械手1X轴报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:20:39'),
	(271, 'm_inX1AlarmOn', 1, '0', '机械手1X轴报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:20:52'),
	(272, 'm_inY1Alarm', 1, '3', '机械手1Y轴报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:20:54'),
	(273, 'm_inY1AlarmOn', 1, '0', '机械手1Y轴报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:21:09'),
	(274, 'm_inX2Alarm', 1, '1', '机械手2X轴报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 22:08:35'),
	(275, 'm_inX2AlarmOn', 1, '0', '机械手2X轴报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:21:17'),
	(276, 'm_inY2Alarm', 1, '4', '机械手2Y轴报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:21:20'),
	(277, 'm_inY2AlarmOn', 1, '0', '机械手2Y轴报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:21:26'),
	(278, 'm_inNgAlarm', 1, '23', '分拣机械手报警输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:21:29'),
	(279, 'm_inNgAlarmOn', 1, '0', '分拣机械手报警触发', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:22:04'),
	(280, 'm_inIn1', 1, '15', '入料感应器1信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:48'),
	(281, 'm_inIn2', 1, '30', '入料感应器2信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-05 14:14:58'),
	(282, 'm_inIn3', 1, '31', '入料感应器3信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:54'),
	(283, 'm_inOut1', 1, '32', '出料皮带感应器信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:05:59'),
	(284, 'm_outBoxPush', 1, '23', 'NG料盒气缸输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:24:14'),
	(285, 'm_ptDistance1[0].X', 1, '-5058', '机械手1取料点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 11:18:07'),
	(286, 'm_ptDistance1[0].Y', 1, '-23216', '机械手1取料点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 16:52:45'),
	(287, 'm_ptDistance1[1].X', 1, '-25119', '机械手1检测点AX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 16:25:10'),
	(288, 'm_ptDistance1[1].Y', 1, '-24998', '机械手1检测点AY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:43:08'),
	(289, 'm_ptDistance1[2].X', 1, '-31153', '机械手1检测点BX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 16:25:10'),
	(290, 'm_ptDistance1[2].Y', 1, '-33344', '机械手1检测点BY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 16:53:11'),
	(291, 'm_ptDistance1[3].X', 1, '-24977', '机械手1检测点CX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 11:29:01'),
	(292, 'm_ptDistance1[3].Y', 1, '-33175', '机械手1检测点CY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:43:43'),
	(293, 'm_ptDistance1[4].X', 1, '-31298', '机械手1检测点DX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:22:41'),
	(294, 'm_ptDistance1[4].Y', 1, '-25056', '机械手1检测点DY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:43:58'),
	(295, 'm_ptDistance1[5].X', 1, '-48598', '机械手1放料点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-27 15:47:13'),
	(296, 'm_ptDistance1[5].Y', 1, '-25459', '机械手1放料点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-27 15:47:13'),
	(297, 'm_ptDistance1[6].X', 1, '-48598', '机械手1收臂点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-27 15:47:13'),
	(298, 'm_ptDistance1[6].Y', 1, '0', '机械手1收臂点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:49:10'),
	(299, 'm_ptDistance1[7].X', 1, '-5058', '机械手1待命点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 11:18:08'),
	(300, 'm_ptDistance1[7].Y', 1, '0', '机械手1待命点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:52:27'),
	(301, 'm_ptDistance2[0].X', 1, '-3893', '机械手2取料点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 11:22:59'),
	(302, 'm_ptDistance2[0].Y', 1, '-31137', '机械手2取料点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 18:47:40'),
	(303, 'm_ptDistance2[1].X', 1, '-23868', '机械手2检测点AX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 18:37:32'),
	(304, 'm_ptDistance2[1].Y', 1, '-28851', '机械手2检测点AY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-21 12:56:00'),
	(305, 'm_ptDistance2[2].X', 1, '-30062', '机械手2检测点BX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 17:29:15'),
	(306, 'm_ptDistance2[2].Y', 1, '-20602', '机械手2检测点BY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-21 13:01:58'),
	(307, 'm_ptDistance2[3].X', 1, '-23845', '机械手2检测点CX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:45:39'),
	(308, 'm_ptDistance2[3].Y', 1, '-20691', '机械手2检测点CY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:33:01'),
	(309, 'm_ptDistance2[4].X', 1, '-30132', '机械手2检测点DX轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:45:53'),
	(310, 'm_ptDistance2[4].Y', 1, '-28815', '机械手2检测点DY轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 12:45:54'),
	(311, 'm_ptDistance2[5].X', 1, '-47528', '机械手2放料点X坐标轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:09:34'),
	(312, 'm_ptDistance2[5].Y', 1, '-28903', '机械手2放料点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:09:41'),
	(313, 'm_ptDistance2[6].X', 1, '-47528', '机械手2收臂点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:09:47'),
	(314, 'm_ptDistance2[6].Y', 1, '0', '机械手2收臂点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:49:58'),
	(315, 'm_ptDistance2[7].X', 1, '-3893', '机械手2待命点X轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 11:22:59'),
	(316, 'm_ptDistance2[7].Y', 1, '0', '机械手2待命点Y轴位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-10-12 22:49:54'),
	(317, 'm_lNgWaitPosition', 1, '4299', '分拣机械手待命点位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:10:02'),
	(318, 'm_lNgPutPosition1', 1, '78348', '分拣机械手放料点1位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:10:09'),
	(319, 'm_lNgPutPosition2', 1, '121270', '分拣机械手放料点2位置', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:10:16'),
	(320, 'm_dOutStepTemp', 1, '235', '出料皮带步距', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:10:20'),
	(321, 'm_inZ2Vacuum', 1, '25', '机械手2真空输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:30:45'),
	(322, 'm_outZ2Vacuum', 1, '30', '机械手2真空输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:31:05'),
	(323, 'm_outZ2UpDown', 1, '21', '机械手2气缸输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:31:46'),
	(324, 'm_inZ1Vacuum', 1, '17', '机械手1真空输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:32:56'),
	(325, 'm_outZ1Vacuum', 1, '26', '机械手1真空输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:33:00'),
	(326, 'm_outZ1UpDown', 1, '20', '机械手1气缸输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:33:23'),
	(327, 'm_inZ3Vacuum', 1, '16', '分拣机械手真空输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:33:52'),
	(328, 'm_outZ3Vacuum', 1, '28', '分拣机械手真空输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:33:56'),
	(329, 'm_outZ3UpDown', 1, '31', '分拣机械手气缸输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:34:17'),
	(330, 'm_inX1Home', 1, '1', '机械手1X轴原点输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 23:29:15'),
	(331, 'm_inX2Home', 1, '3', '机械手2X轴原点输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:34:49'),
	(332, 'm_inY1Home', 1, '5', '机械手1Y轴原点输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:34:52'),
	(333, 'm_inY2Home', 1, '7', '机械手2Y轴原点输入信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:35:36'),
	(334, 'm_outX1Home', 1, '1', '机械手1X轴原点输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 23:32:05'),
	(335, 'm_outX2Home', 1, '2', '机械手2X轴原点输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:35:40'),
	(336, 'm_outY1Home', 1, '3', '机械手1Y轴原点输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:35:42'),
	(337, 'm_outY2Home', 1, '4', '机械手2Y轴原点输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:36:00'),
	(338, 'm_iBlockTime', 1, '5', '堵料时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-01 10:42:46'),
	(339, 'm_iNGBoxNum', 1, '9', 'NG料盒满料数量', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-01 10:42:48'),
	(340, 'm_iCaptureTime', 1, '150', '采图延时', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-23 14:44:53'),
	(341, 'm_dNgKeepDown', 1, '0.05', '分拣机械手下降保持时间', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-23 14:50:51'),
	(342, 'm_iRayMoveEnable', 1, '0', '光管运动使能', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-12 10:37:18'),
	(343, 'm_iEnableEndSensor', 1, '0', '末端感应器启用', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-01 10:43:01'),
	(344, 'm_iConnectMachine', 1, '0', '联机使能', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-01 10:43:06'),
	(345, 'm_iInspectType', 1, '1', '检测模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-10-10 23:39:06'),
	(346, 'm_outInAllow', 1, '25', '允许上料输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-13 17:40:53'),
	(347, 'RedLight', 1, '14', '三色灯红灯输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-13 23:34:00'),
	(348, 'YellowLight', 1, '12', '三色灯黄灯输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-15 10:31:04'),
	(349, 'GreenLight', 1, '11', '三色灯绿灯输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-15 10:31:06'),
	(350, 'Buzzer', 1, '14', '蜂鸣器输出信号', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-13 23:34:12'),
	(351, 'EquipmentID', 1, 'P8-1021', '机台编号', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-20 10:10:52'),
	(352, 'mesImgSavePath', 1, '//nd-x-ray/X-RAY', 'MES下发图片路径', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-13 18:33:52'),
	(353, 'voltage', 1, '40.1', '电压', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-12 14:17:59'),
	(354, 'voltage_up', 1, '63.2', '电压', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-04 22:54:31'),
	(355, 'voltage_down', 1, '26.3', '电压', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-07 14:56:46'),
	(356, 'current', 1, '27.1', '电流', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-12 14:17:59'),
	(357, 'current_up', 1, '40.2', '电流', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-03 14:48:43'),
	(358, 'current_down', 1, '20.3', '电流', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-07 14:56:46'),
	(359, 'GG00_expect', 1, '200', '光管寿命', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-12 14:17:47'),
	(360, 'GG00_curr', 1, '0', '光管寿命_当前', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-13 23:16:39'),
	(361, 'ZQQ0_expect', 1, '100', '增强器寿命', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-12 14:18:31'),
	(362, 'ZQQ0_curr', 1, '0', '增强器寿命_当前', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-13 23:16:39'),
	(363, 'ParentEQState', 1, 'Stop', 'Equipment State', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(364, 'AndonState', 1, 'Stop', 'Andon State', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(365, 'Quantity', 1, '24813', 'Current Quantity', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(366, 'IsRecheckMode', 1, '0', '是否为人工复判模式', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-28 18:33:04'),
	(556, 'RetestSpec', 1, '0.13', '阴阳极错位距离复判规格', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-31 14:23:13'),
	(557, 'RetestSpec_up', 1, '1.85', '阴阳极错位距离复判规格上限', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-31 14:15:40'),
	(558, 'RetestSpec_down', 1, '0.13', '阴阳极错位距离复判规格下限', 'Float', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:51'),
	(559, 'TestSpec', 1, '0.5', '阴阳极错位距离', 'Float', 1, NULL, NULL, b'0', NULL, '2020-03-29 10:47:35'),
	(560, 'TestSpec_up', 1, '1.7', '阴阳极错位距离上限', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-31 14:15:40'),
	(561, 'TestSpec_down', 1, '0.2', '阴阳极错位距离下限', 'Float', 1, NULL, NULL, b'0', NULL, '2019-12-31 14:15:40'),
	(562, 'LayerACNum', 1, '9', '层数A/C', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-31 14:40:21'),
	(563, 'LayerACNum_up', 1, '0', '层数A/C上限', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:52'),
	(564, 'LayerACNum_down', 1, '0', '层数A/C下限', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:52'),
	(565, 'LayerBDNum', 1, '7', '层数B/D', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:53'),
	(566, 'LayerBDNum_up', 1, '0', '层数B/D上限', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:52'),
	(567, 'LayerBDNum_down', 1, '0', '层数B/D下限', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-11-24 16:28:52'),
	(568, 'EnableInputFloat', 1, '0', '是否启用Input下发参数Float', 'Float', 1, NULL, NULL, b'0', NULL, '2020-03-29 10:47:35'),
	(569, 'EnableInputInt', 1, '0', '是否启用Input下发参数Int', 'Int32', 1, NULL, NULL, b'0', NULL, '2020-03-29 10:47:35'),
	(570, 'AlgoVersionInput', 1, 'V0.19.12.27', 'MES下发的算法版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-31 12:17:31'),
	(571, 'AlgoFilePath', 1, 'E:/zy_Xray_inspection.dll', 'MES下发的算法文件路径', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-02 10:30:58'),
	(572, 'AlgoVersionLocal', 1, 'V0.19.12.27', '当前的算法版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-31 12:16:29'),
	(599, 'AutoPopMonitorPage', 1, '1', 'Auto Pop Monitor Page', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(605, 'LabChinese', 1, 'FEF', 'LabChinese', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(606, 'LabEnglish', 1, 'ALI-FEF', 'LabEnglish', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(607, 'LabVersion', 1, 'V1.1.0001', 'App Version', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03'),
	(611, 'DefaultAcount', 1, 'superAdmin', 'Default Acount', 'String', 1, NULL, NULL, b'0', NULL, '2020-09-03 09:55:03');
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
