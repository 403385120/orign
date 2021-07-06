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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COMMENT='报警记录';

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
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='dispose_state = 0 表示暂未处理的报警\r\ndispose_state = 1 表示暂已处理的报警\r\nduration表示报警时长(min)';

-- Dumping data for table ptf.alarm_temp: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_temp` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_temp` ENABLE KEYS */;

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

-- Dumping data for table ptf.device_alert_config: ~19 rows (大约)
/*!40000 ALTER TABLE `device_alert_config` DISABLE KEYS */;
INSERT INTO `device_alert_config` (`EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `AlertLevel`, `AlertBitAddr`, `DataTime`) VALUES
	('ACOA0022', 1, 'Alarm001', '上料输送带电芯追尾(R13/R14），请检查！', 'A', 'D15220.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm002', '备用61001', 'A', 'D15220.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm003', '左切边防切爆感应异常(R203）,请取走电芯并清除该工位电芯记忆！', 'A', 'D15220.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm004', '左切刀使用寿命已到,请更换切刀！', 'A', 'D15220.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm005', '左折角光纤检测异常(R203),请取走电芯并清除该工位电芯记忆！', 'A', 'D15220.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm006', '左滴胶感应异常(R300),请取走电芯左精整位并清除该工位电芯记忆！', 'A', 'D15220.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm007', '左换胶时间已到,请更换,并切换到左滴胶操作页面清零计时！', 'A', 'D15220.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm008', '换向锁紧防呆感应异常(R406),请检查左冷烫位夹具是否合上！', 'A', 'D15220.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm009', '右切边光纤感应异常(R800),请取走电芯并清除该工位电芯记忆！', 'A', 'D15220.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm010', '右切刀使用寿命已到,请更换切刀！', 'A', 'D15220.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm011', '右折角感应异常(R803),请取走电芯并清除该工位电芯记忆！', 'A', 'D15220.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm012', '右滴胶感应异常(R814),请取走电芯右精整位并清除该工位电芯记忆！', 'A', 'D15220.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm013', '右换胶时间已到,并切换到左滴胶操作页面清零计时！！', 'A', 'D15220.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm014', '下料推电芯感应异常（R1002），请手动纠正电芯位置！', 'A', 'D15220.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm015', '下料夹具开合防呆感应异常(R1001),请检查右冷烫位夹具是否合上！', 'A', 'D15220.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm016', '连续3个扫码不良报警', 'A', 'D15220.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm017', '上料翻转位有电池（R101），请手动取走并关闭真空阀！', 'A', 'D15221.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm018', '上料搬运A位有电池（R112），请手动取走并关闭真空阀！', 'A', 'D15221.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm019', '上料搬运B位有电池（R113），请手动取走并关闭真空阀！', 'A', 'D15221.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm020', '备用61103', 'A', 'D15221.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm021', '备用61104', 'A', 'D15221.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm022', '备用61105', 'A', 'D15221.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm023', '备用61106', 'A', 'D15221.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm024', '备用61107', 'A', 'D15221.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm025', '备用61108', 'A', 'D15221.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm026', '备用61109', 'A', 'D15221.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm027', '备用61110', 'A', 'D15221.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm028', '备用61111', 'A', 'D15221.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm029', '备用61112', 'A', 'D15221.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm030', '备用61113', 'A', 'D15221.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm031', '备用61114', 'A', 'D15221.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm032', '备用61115', 'A', 'D15221.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm033', 'A-1上料翻转伺服电机限位开关(LSF)报警！', 'A', 'D15222.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm034', 'A-1上料翻转伺服电机限位开关(LSR)报警！', 'A', 'D15222.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm035', 'A-2裙边整形伺服电机限位开关(LSF)报警！', 'A', 'D15222.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm036', 'A-2裙边整形伺服电机限位开关(LSR)报警！', 'A', 'D15222.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm037', 'A-3上料搬运平移伺服电机限位开关(LSF)报警！', 'A', 'D15222.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm038', 'A-3上料搬运平移伺服电机限位开关(LSR)报警！', 'A', 'D15222.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm039', 'A-4上料搬运升降伺服电机限位开关(LSF)报警！', 'A', 'D15222.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm040', 'A-4上料搬运升降伺服电机限位开关(LSR)报警！', 'A', 'D15222.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm041', 'A-5上料推电芯伺服电机限位开关(LSF)报警！', 'A', 'D15222.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm042', 'A-5上料推电芯伺服电机限位开关(LSR)报警！', 'A', 'D15222.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm043', 'A-6上料锁紧伺服电机限位开关(LSF)报警！', 'A', 'D15222.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm044', 'A-6上料锁紧伺服电机限位开关(LSR)报警！', 'A', 'D15222.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm045', 'A-7上料定位伺服电机限位开关(LSF)报警！', 'A', 'D15222.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm046', 'A-7上料定位伺服电机限位开关(LSR)报警！', 'A', 'D15222.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm047', 'A-8左折角伺服电机限位开关(LSF)报警！', 'A', 'D15222.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm048', 'A-8左折角伺服电机限位开关(LSR)报警！', 'A', 'D15222.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm049', 'A-9中间拨快1伺服电机限位开关(LSF)报警！', 'A', 'D15223.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm050', 'A-9中间拨快1伺服电机限位开关(LSR)报警！', 'A', 'D15223.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm051', 'A-10中间拨快2伺服电机限位开关(LSF)报警！', 'A', 'D15223.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm052', 'A-10中间拨快2伺服电机限位开关(LSR)报警！', 'A', 'D15223.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm053', 'A-11中间拨快3伺服电机限位开关(LSF)报警！', 'A', 'D15223.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm054', 'A-11中间拨快3伺服电机限位开关(LSR)报警！', 'A', 'D15223.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm055', 'A-12中间拨快4伺服电机限位开关(LSF)报警！', 'A', 'D15223.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm056', 'A-12中间拨快4伺服电机限位开关(LSR)报警！', 'A', 'D15223.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm057', 'A-13中间拨快5伺服电机限位开关(LSF)报警！', 'A', 'D15223.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm058', 'A-13中间拨快5伺服电机限位开关(LSR)报警！', 'A', 'D15223.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm059', 'A-14换向锁紧伺服电机限位开关(LSF)报警！', 'A', 'D15223.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm060', 'A-14换向锁紧伺服电机限位开关(LSR)报警！', 'A', 'D15223.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm061', 'A-15换向定位伺服电机限位开关(LSF)报警！', 'A', 'D15223.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm062', 'A-15换向定位伺服电机限位开关(LSR)报警！', 'A', 'D15223.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm063', 'A-16 后转运伺服电机限位开关(LSF)报警！', 'A', 'D15223.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm064', 'A-16 后转运伺服电机限位开关(LSR)报警！', 'A', 'D15223.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm065', 'B-1 右折角伺服电机限位开关(LSF)报警！', 'A', 'D15224.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm066', 'B-1 右折角伺服电机限位开关(LSR)报警！', 'A', 'D15224.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm067', 'B-2 右折边前后伺服电机限位开关(LSF)报警！', 'A', 'D15224.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm068', 'B-2 右折边前后伺服电机限位开关(LSR)报警！', 'A', 'D15224.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm069', 'B-3 右一折边伺服电机限位开关(LSF)报警！', 'A', 'D15224.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm070', 'B-3 右一折边伺服电机限位开关(LSR)报警！', 'A', 'D15224.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm071', 'B-4 中间拨快6伺服电机限位开关(LSF)报警！', 'A', 'D15224.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm072', 'B-4 中间拨快6伺服电机限位开关(LSR)报警！', 'A', 'D15224.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm073', 'B-5 中间拨快7伺服电机限位开关(LSF)报警！', 'A', 'D15224.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm074', 'B-5 中间拨快7伺服电机限位开关(LSR)报警！', 'A', 'D15224.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm075', '备用61410', 'A', 'D15224.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm076', '备用61411', 'A', 'D15224.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm077', '备用61412', 'A', 'D15224.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm078', '备用61413', 'A', 'D15224.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm079', '备用61414', 'A', 'D15224.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm080', '备用61415', 'A', 'D15224.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm081', 'B-9 中间拨快8伺服电机限位开关(LSF)报警！', 'A', 'D15225.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm082', 'B-9 中间拨快8伺服电机限位开关(LSR)报警！', 'A', 'D15225.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm083', 'B-10 下料开夹伺服电机限位开关(LSF)报警！', 'A', 'D15225.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm084', 'B-10 下料开夹伺服电机限位开关(LSR)报警！', 'A', 'D15225.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm085', 'B-11 下料推电芯伺服电机限位开关(LSF)报警！', 'A', 'D15225.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm086', 'B-11 下料推电芯伺服电机限位开关(LSR)报警！', 'A', 'D15225.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm087', 'B-12 下料搬运平移伺服电机限位开关(LSF)报警！', 'A', 'D15225.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm088', 'B-12 下料搬运平移伺服电机限位开关(LSR)报警！', 'A', 'D15225.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm089', 'B-13 下料升降伺服电机限位开关(LSF)报警！', 'A', 'D15225.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm090', 'B-13 下料升降伺服电机限位开关(LSR)报警！', 'A', 'D15225.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm091', 'B-14 前转运伺服电机限位开关(LSF)报警！', 'A', 'D15225.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm092', 'B-14 前转运伺服电机限位开关(LSR)报警！', 'A', 'D15225.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm093', 'B-15 下料翻转平移伺服电机限位开关(LSF)报警！', 'A', 'D15225.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm094', 'B-15 下料翻转平移伺服电机限位开关(LSR)报警！', 'A', 'D15225.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm095', 'B-16 下料翻转平移伺服电机限位开关(LSF)报警！', 'A', 'D15225.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm096', 'B-16 下料翻转升降伺服电机限位开关(LSR)报警！', 'A', 'D15225.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm097', '左控制面板急停(I:R005)已按下请注意安全！', 'A', 'D15226.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm098', '清夹具急停(I:R009)已按下请注意安全！', 'A', 'D15226.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm099', '右控制面板急停(I:R806)已按下请注意安全！', 'A', 'D15226.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm100', '下料急停(I:R708)已按下请注意安全！', 'A', 'D15226.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm101', '前安全(I:R007)已打开请注意安全！', 'A', 'D15226.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm102', '左安全(I:R008)已打开请注意安全！', 'A', 'D15226.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm103', '后安全(I:R709)已打开请注意安全！', 'A', 'D15226.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm104', '右安全(I:R710)已打开请注意安全！', 'A', 'D15226.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm105', '上料预定位气缸(Q:R1607)缩回(I:R0015)报警！', 'A', 'D15226.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm106', '上料预定位气缸(Q:R1607)伸出(I:R0100)报警！', 'A', 'D15226.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm107', '备用61610', 'A', 'D15226.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm108', '备用61611', 'A', 'D15226.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm109', '上料翻转气缸旋(Q:R1608)原位(I:R0102)报警！', 'A', 'D15226.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm110', '上料翻转气缸旋(Q:R1609)上旋转位(I:R0103)报警！', 'A', 'D15226.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm111', '上料翻转(Q:R1610)真空(I:R0101)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15226.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm112', '上料翻转(Q:R1610)破真空(I:R0101)报警，请检查！', 'A', 'D15226.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm113', '裙边整形气缸(Q:R1611)上限(I:R0104)报警！', 'A', 'D15227.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm114', '裙边整形气缸(Q:R1611)下限(I:R0105)报警！', 'A', 'D15227.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm115', '裙边整形气缸(Q:R1612)上限位(I:R0106)报警！', 'A', 'D15227.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm116', '裙边整形气缸(Q:R1612)下限位(I:R0107)报警！', 'A', 'D15227.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm117', '裙边整形左上加热(Q:R1613)(I:R0108)报警！', 'A', 'D15227.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm118', '备用61705', 'A', 'D15227.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm119', '裙边整形右上加热(Q:R1614)(I:R0109)报警！', 'A', 'D15227.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm120', '备用61707', 'A', 'D15227.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm121', '上料搬运换向气缸(Q:R1615)原位(I:R0110)报警！', 'A', 'D15227.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm122', '上料搬运换向气缸(Q:R1615)旋转位(I:R0111)报警！', 'A', 'D15227.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm123', '上料搬运A(Q:R1700)真空(I:R0112)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15227.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm124', '上料搬运A(Q:R1700)破真空(I:R0112)报警，请检查！', 'A', 'D15227.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm125', '备用61712', 'A', 'D15227.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm126', '备用61713', 'A', 'D15227.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm127', '上料搬运B(Q:R1702)真空(I:R0113)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15227.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm128', '上料搬运B(Q:R1702)破真空(I:R0113)，请检查！', 'A', 'D15227.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm129', '备用61800', 'A', 'D15228.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm130', '备用61801', 'A', 'D15228.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm131', '左切边上下气缸(Q:R1704)上限(I:R0201)报警！', 'A', 'D15228.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm132', '左切边上下气缸(Q:R1704)下限(I:R0202)报警！', 'A', 'D15228.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm133', '备用61804', 'A', 'D15228.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm134', '备用61805', 'A', 'D15228.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm135', '左折角上压紧气缸(Q:R1706)上限(I:R0204)报警！', 'A', 'D15228.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm136', '左折角上压紧气缸(Q:R1706)(I:R0204)未灭报警！', 'A', 'D15228.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm137', '左折角下折刀气缸(Q:R1707)下限(I:R0205)报警！', 'A', 'D15228.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm138', '左折角下折刀气缸(Q:R1707)下限(I:R0208)未灭报警！', 'A', 'D15228.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm139', '左折角平移气缸(Q:R1708)缩回(I:R0206)报警！', 'A', 'D15228.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm140', '左折角平移气缸(Q:R1708)伸出(I:R0207)报警！', 'A', 'D15228.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm141', '左折角平推气缸(Q:R1709)缩回(I:R0208)报警！', 'A', 'D15228.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm142', '左折角平推气缸(Q:R1709)伸出(I:R0209)报警！', 'A', 'D15228.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm143', '左1折边气缸(Q:R1710)上限(I:R0210)报警！', 'A', 'D15228.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm144', '左1折边气缸(Q:R1710)下限(I:R0211)报警！', 'A', 'D15228.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm145', '左1折热压气缸(Q:R1711)上限(I:R0212)报警！', 'A', 'D15229.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm146', '左1折热压气缸(Q:R1711)下限(I:R0213)报警！', 'A', 'D15229.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm147', '左1折热压上加热(Q:R1712)(I:R0214)报警！', 'A', 'D15229.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm148', '备用61903', 'A', 'D15229.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm149', '左1折热压下加热(Q:R1713)(I:R0215)报警！', 'A', 'D15229.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm150', '备用61905', 'A', 'D15229.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm151', '左滴胶气缸(Q:R1714)缩回(I:R0301)报警！', 'A', 'D15229.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm152', '左滴胶气缸(Q:R1714)伸出(I:R0302)报警！', 'A', 'D15229.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm153', '备用61908', 'A', 'D15229.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm154', '备用61909', 'A', 'D15229.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm155', '左2折边气缸(Q:R1800)缩回(I:R0303)报警！', 'A', 'D15229.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm156', '左2折边气缸(Q:R1800)伸出(I:R0304)报警！', 'A', 'D15229.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm157', '左精整平推气缸(Q:R1801)缩回(I:R0305)报警！', 'A', 'D15229.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm158', '左精整平推气缸(Q:R1801)伸出(I:R0306)报警！', 'A', 'D15229.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm159', '左精整上下气缸（Q:R1802)上限(I:R0307)报警！', 'A', 'D15229.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm160', '左精整上下气缸（Q:R1802)下限(I:R0308)报警！', 'A', 'D15229.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm161', '左2折热烫气缸A(Q:R1803)缩回(I:R0309)报警！', 'A', 'D15230.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm162', '左2折热烫气缸A(Q:R1803)伸出(I:R0310)报警！', 'A', 'D15230.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm163', '左2折热烫加热A(Q:R1804)缩回(I:R0311)报警！', 'A', 'D15230.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm164', '备用62003', 'A', 'D15230.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm165', '左2折热烫气缸B(Q:R1805)缩回(I:R0312)报警！', 'A', 'D15230.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm166', '左2折热烫气缸B(Q:R1805)伸出(I:R0313)报警！', 'A', 'D15230.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm167', '左2折热烫加热B（Q:R1806)(I:R0314)报警！', 'A', 'D15230.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm168', '备用62007', 'A', 'D15230.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm169', '左2折冷烫气缸A(Q:R1807)缩回(I:R0315)报警！', 'A', 'D15230.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm170', '左2折冷烫气缸A(Q:R1807)伸出(I:R0400)报警！', 'A', 'D15230.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm171', '左2折冷烫气缸B(Q:R1808)缩回(I:R0401)报警！', 'A', 'D15230.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm172', '左2折冷烫气缸B(Q:R1808)伸出(I:R0402)报警！', 'A', 'D15230.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm173', '左水平精整气缸(Q:R1809)缩回(I:R0403)报警！', 'A', 'D15230.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm174', '左水平精整气缸(Q:R1809)伸出(I:R0404)报警！', 'A', 'D15230.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm175', '换向定位气缸(Q:R1810)缩回(I:R0407)报警！', 'A', 'D15230.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm176', '换向定位气缸(Q:R1810)伸出(I:R0408)报警！', 'A', 'D15230.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm177', '中间拨快2气缸Q:R1811)缩回(I:R0409)报警！', 'A', 'D15231.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm178', '中间拨快2气缸(Q:R1811)伸出(I:R0410)报警！', 'A', 'D15231.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm179', '中间拨快3气缸(Q:R1812)缩回(I:R0411)报警！', 'A', 'D15231.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm180', '中间拨快3气缸(Q:R1812)伸出(I:R0412)报警！', 'A', 'D15231.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm181', '中间拨快4左气缸(Q:R1813)缩回(I:R0413)报警！', 'A', 'D15231.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm182', '中间拨快4左气缸(Q:R1813)伸出(I:R0414)报警！', 'A', 'D15231.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm183', '中间拨快4右气缸(Q:R1813)缩回(I:R0415)报警！', 'A', 'D15231.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm184', '中间拨快4右气缸(Q:R1813)伸出(I:R0500)报警！', 'A', 'D15231.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm185', '中间拨快5左气缸(Q:R1814)缩回(I:R0501)报警！', 'A', 'D15231.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm186', '中间拨快5左气缸(Q:R1814)伸出(I:R0502)报警！', 'A', 'D15231.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm187', '夹具中间拨快5右(Q:R1815)缩回(I:R0503)报警！', 'A', 'D15231.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm188', '中间拨快5右(Q:R1815)伸出(I:R0504)报警！', 'A', 'D15231.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm189', '上料锁紧夹具固定气缸1(Q:R1900)上限(I:R0602)报警！', 'A', 'D15231.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm190', '上料锁紧夹具固定气缸1(Q:R1900)下限(I:R0602)报警！', 'A', 'D15231.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm191', '左切边夹具固定气缸2(Q:R1901)上限(I:R0603)报警！', 'A', 'D15231.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm192', '左切边夹具固定气缸2(Q:R1901)下限(I:R0603)报警！', 'A', 'D15231.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm193', '左折角夹具固定气缸3(Q:R1902)上限(I:R0604)报警！', 'A', 'D15232.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm194', '左折角夹具固定气缸3(Q:R1902)下限(I:R0604)报警！', 'A', 'D15232.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm195', '左1折热压夹具固定气缸4(Q:R1903)上限(I:R0605)报警！', 'A', 'D15232.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm196', '左1折热压夹具固定气缸4(Q:R1903)下限(I:R0605)报警！', 'A', 'D15232.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm197', '左滴胶夹具固定气缸5(Q:R1904)上限(I:R0606)报警！', 'A', 'D15232.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm198', '左滴胶夹具固定气缸5(Q:R1904)下限(I:R0606)报警！', 'A', 'D15232.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm199', '左精整夹具固定气缸6(Q:R1905)上限(I:R0607)报警！', 'A', 'D15232.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm200', '左精整夹具固定气缸6(Q:R1905)下限(I:R0607)报警！', 'A', 'D15232.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm201', '左2折热烫A夹具固定气缸7(Q:R1906)上限(I:R0608)报警！', 'A', 'D15232.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm202', '左2折热烫A夹具固定气缸7(Q:R1906)下限(I:R0608)报警！', 'A', 'D15232.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm203', '左2折热烫B夹具固定气缸8(Q:R1907)上限(I:R0609)报警！', 'A', 'D15232.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm204', '左2折热烫B夹具固定气缸8(Q:R1907)下限(I:R0609)报警！', 'A', 'D15232.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm205', '左2折冷烫A夹具固定气缸9(Q:R1908)上限(I:R0610)报警！', 'A', 'D15232.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm206', '左2折冷烫A夹具固定气缸9(Q:R1908)下限(I:R0610)报警！', 'A', 'D15232.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm207', '左2折冷烫B夹具固定气缸10(Q:R1909)上限(I:R0611)报警！', 'A', 'D15232.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm208', '左2折冷烫B夹具固定气缸10(Q:R1909)下限(I:R0611)报警！', 'A', 'D15232.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm209', '左水平精整夹具固定气缸11(Q:R1910)上限(I:R0612)报警！', 'A', 'D15233.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm210', '左水平精整夹具固定气缸11(Q:R1910)下限(I:R0612)报警！', 'A', 'D15233.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm211', '换向锁紧夹具固定气缸12(Q:R1911)上限(I:R0613)报警！', 'A', 'D15233.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm212', '换向锁紧夹具固定气缸12(Q:R1911)下限(I:R0613)报警！', 'A', 'D15233.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm213', '后转运夹具固定气缸13(Q:R1912)上限(I:R0614)报警！', 'A', 'D15233.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm214', '后转运夹具固定气缸13(Q:R1912)下限(I:R0614)报警！', 'A', 'D15233.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm215', '备用62306', 'A', 'D15233.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm216', '备用62307', 'A', 'D15233.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm217', '备用62308', 'A', 'D15233.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm218', '备用62309', 'A', 'D15233.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm219', '备用62310', 'A', 'D15233.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm220', '备用62311', 'A', 'D15233.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm221', '备用62312', 'A', 'D15233.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm222', '备用62313', 'A', 'D15233.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm223', '右切边上下气缸(Q:R2000)上限(I:R0801)报警！', 'A', 'D15233.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm224', '右切边上下气缸(Q:R2000)下限(I:R0802)报警！', 'A', 'D15233.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm225', '右折角上压紧气缸(Q:R2002)上限(I:R0804)报警！', 'A', 'D15234.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm226', '备用62401', 'A', 'D15234.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm227', '右折角上压紧气缸(Q:R2002)上限(I:R0804)报警！', 'A', 'D15234.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm228', '右折角上压紧气缸(Q:R2002)下限(I:R0804)报警！', 'A', 'D15234.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm229', '右折角下折刀气缸(Q:R2003)上限(I:R0805)报警！', 'A', 'D15234.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm230', '右折角下折刀气缸(Q:R2003)下限(I:R0805)报警！', 'A', 'D15234.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm231', '右折角平移气缸(Q:R2004)缩回(I:R0806)报警！', 'A', 'D15234.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm232', '右折角平移气缸(Q:R2004)伸出(I:R0807)', 'A', 'D15234.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm233', '右折角平推气缸(Q:R2005)伸出(I:R0809)报警！', 'A', 'D15234.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm234', '右折角平推气缸(Q:R2005)缩回(I:R0808)', 'A', 'D15234.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm235', '右1折热压气缸(Q:R2006)上限(I:R0810)报警！', 'A', 'D15234.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm236', '右1折热压气缸(Q:R2006)下限(I:R0811)报警！', 'A', 'D15234.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm237', '右1折热压上加热(Q:R2007)(I:R0812)报警！', 'A', 'D15234.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm238', '备用62413', 'A', 'D15234.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm239', '右1折热压下加热(Q:R2008)(I:R0813)报警！', 'A', 'D15234.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm240', '备用62415', 'A', 'D15234.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm241', '右滴胶气缸(Q:R2009)缩回(I:R0815)报警！', 'A', 'D15235.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm242', '右滴胶气缸(Q:R2009)伸出(I:R0900)报警！', 'A', 'D15235.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm243', '备用62502', 'A', 'D15235.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm244', '备用62503', 'A', 'D15235.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm245', '右2折边气缸(Q:R2011)缩回(I:R0901)报警！', 'A', 'D15235.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm246', '右2折边气缸(Q:R2011)伸出(I:R0902)报警！', 'A', 'D15235.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm247', '右精整平推气缸(Q:R2012)缩回(I:R0903)报警！', 'A', 'D15235.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm248', '右精整平推气缸(Q:R2012)伸出(I:R0904)报警！', 'A', 'D15235.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm249', '右精整上下气缸(Q:R2013)上限(I:R0905)报警！', 'A', 'D15235.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm250', '右精整上下气缸(Q:R2013)下限(I:R0906)报警！', 'A', 'D15235.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm251', '右2折热烫气缸A(Q:R2014)缩回(I:R0907)报警！', 'A', 'D15235.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm252', '右2折热烫气缸A(Q:R2014)伸出(I:R0908)报警！', 'A', 'D15235.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm253', '右2折热烫加热A(Q:R2015)(I:R0909)报警！', 'A', 'D15235.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm254', '备用62513', 'A', 'D15235.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm255', '右2折热烫气缸B(Q:R2100)缩回(I:R0910)报警！', 'A', 'D15235.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm256', '右2折热烫气缸B(Q:R2100)伸出(I:R0911)报警！', 'A', 'D15235.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm257', '右2折热烫加热B(Q:R2101)(I:R0912)报警！', 'A', 'D15236.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm258', '备用62601', 'A', 'D15236.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm259', '右2折冷烫气缸A(Q:R2102)缩回(I:R0913)报警！', 'A', 'D15236.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm260', '右2折冷烫气缸A(Q:R2102)伸出(I:R0914)报警！', 'A', 'D15236.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm261', '右水平精整气缸(Q:R2103)缩回(I:R0915)报警！', 'A', 'D15236.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm262', '右水平精整气缸(Q:R2103)伸出(I:R01000)报警！', 'A', 'D15236.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm263', '备用62606', 'A', 'D15236.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm264', '备用62607', 'A', 'D15236.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm265', '右2折冷烫气缸B(Q:R2104)缩回(I:R1004)报警！', 'A', 'D15236.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm266', '右2折冷烫气缸B(Q:R2104)伸出I:R1005)报警！', 'A', 'D15236.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm267', '右2折冷烫气缸B上下气缸(Q:R2105)上限(I:R1006)报警！', 'A', 'D15236.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm268', '右2折冷烫气缸B上下气缸(Q:R2105)下限(I:R1007)报警！', 'A', 'D15236.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm269', '两侧精整A气缸(Q:R2106)缩回(I:R1008)报警！', 'A', 'D15236.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm270', '两侧精整A气缸(Q:R2106)伸出(I:R1009)报警！', 'A', 'D15236.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm271', '两侧精整A左加热(Q:R2107)(I:R1010)报警！', 'A', 'D15236.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm272', '两侧精整A右加热(Q:R2108)(I:R1011)报警！', 'A', 'D15236.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm273', '两侧精整B气缸(Q:R2109)缩回(I:R1012)报警！', 'A', 'D15237.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm274', '两侧精整B气缸(Q:R2109)伸出(I:R1013)报警！', 'A', 'D15237.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm275', '两侧精整B左加热(Q:R2110)(I:R1014)报警！', 'A', 'D15237.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm276', '备用62703', 'A', 'D15237.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm277', '两侧精整B右加热(Q:R2111)(I:R1015)报警！', 'A', 'D15237.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm278', '备用62705', 'A', 'D15237.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm279', '下料搬运A(Q:R2112)真空(I:R1102)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15237.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm280', '下料搬运A(Q:R2112)破真空(I:R1102)报警，请检查！', 'A', 'D15237.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm281', '下料搬运B(Q:R2114)真空(I:R1103)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15237.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm282', '下料搬运B(Q:R2114)破真空(I:R1103)报警，请检查！', 'A', 'D15237.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm283', '下料搬运C(Q:R2200)真空(I:R1104)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15237.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm284', '下料搬运C(Q:R2200)破真空(I:R1104)报警，请检查！', 'A', 'D15237.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm285', '备用62712', 'A', 'D15237.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm286', '备用62713', 'A', 'D15237.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm287', '备用62714', 'A', 'D15237.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm288', '备用62715', 'A', 'D15237.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm289', '备用62800', 'A', 'D15238.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm290', '备用62801', 'A', 'D15238.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm291', '下料搬运换向气缸(Q:R2202)原位(I:R1100)报警！', 'A', 'D15238.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm292', '下料搬运换向气缸(Q:R2202)旋转位(I:R1101)报警！', 'A', 'D15238.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm293', '备用62804', 'A', 'D15238.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm294', '备用62805', 'A', 'D15238.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm295', '下料翻转气缸(Q:R2203)原位(I:R1105)报警！', 'A', 'D15238.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm296', '下料翻转气缸(Q:R2204)旋转位(I:R1106)报警！', 'A', 'D15238.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm297', '下料翻转真空(Q:R2205)(I:R1107)报警，有F098/F099风险，请找到电芯再开机！', 'A', 'D15238.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm298', '下料翻转(Q:R2205)破真空(I:R1107)报警！', 'A', 'D15238.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm299', '备用62810', 'A', 'D15238.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm300', '备用62811', 'A', 'D15238.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm301', '中间拨快1左气缸(左)(Q:R2207)缩回(I:R1109)报警！', 'A', 'D15238.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm302', '中间拨快1左气缸(左)(Q:R2207)伸出(I:R1110)报警！', 'A', 'D15238.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm303', '中间拨快1右气缸(右）(Q:R2208)缩回(I:R1111)报警！', 'A', 'D15238.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm304', '中间拨快1右气缸(右）(Q:R2208)伸出(I:R1112)报警！', 'A', 'D15238.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm305', '中间拨快6左气缸(Q:R2209)缩回(I:R1115)报警！', 'A', 'D15239.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm306', '中间拨快6右气缸(Q:R2209)伸出(I:R1200)报警！', 'A', 'D15239.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm307', '中间拨快7气缸(Q:R2210)缩回(I:R1201)报警！', 'A', 'D15239.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm308', '中间拨快7气缸(Q:R220910)伸出(I:R1202)报警！', 'A', 'D15239.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm309', '中间拨快8气缸(Q:R22101)缩回(I:R1203)报警！', 'A', 'D15239.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm310', '中间拨快8气缸(Q:R2211)缩回(I:R1204)报警！', 'A', 'D15239.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm311', '右切边夹具固定气缸14(Q:R2212)上限(I:R1215)报警！', 'A', 'D15239.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm312', '右切边夹具固定气缸14(Q:R2212)下限(I:R1301)报警！', 'A', 'D15239.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm313', '右折角夹具固定气缸15(Q:R2213)上限(I:R1302)报警！', 'A', 'D15239.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm314', '右折角夹具固定气缸15(Q:R2213)下限(I:R1302)报警！', 'A', 'D15239.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm315', '右1折边夹具固定气缸16(Q:R2214)上限(I:R1303)报警！', 'A', 'D15239.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm316', '右1折边夹具固定气缸16(Q:R2214)下限(I:R1303)报警！', 'A', 'D15239.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm317', '右1折热压夹具固定气缸17(Q:R2215)上限(I:R1304)报警！', 'A', 'D15239.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm318', '右1折热压夹具固定气缸17(Q:R2215)下限(I:R1304)报警！', 'A', 'D15239.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm319', '右滴胶夹具固定气缸18(Q:R2300)上限(I:R1305)报警！', 'A', 'D15239.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm320', '右滴胶夹具固定气缸18(Q:R2300)下限(I:R1305)报警！', 'A', 'D15239.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm321', '右精整夹具固定气缸19(Q:R2301)上限(I:R1306)报警！', 'A', 'D15240.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm322', '右精整夹具固定气缸19(Q:R2301)下限(I:R1306)报警！', 'A', 'D15240.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm323', '右2折热烫A夹具固定气缸20(Q:R2302)上限(I:R1307)报警！', 'A', 'D15240.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm324', '右2折热烫A夹具固定气缸20(Q:R2302)下限(I:R1307)报警！', 'A', 'D15240.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm325', '右2折热烫B夹具固定气缸21(Q:R2303)上限(I:R1308)报警！', 'A', 'D15240.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm326', '右2折热烫B夹具固定气缸21(Q:R2303)下限(I:R1308)报警！', 'A', 'D15240.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm327', '右2折冷烫A夹具固定气缸22(Q:R2304)上限(I:R1309)报警！', 'A', 'D15240.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm328', '右2折冷烫A夹具固定气缸22(Q:R2304)下限(I:R1309)报警！', 'A', 'D15240.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm329', '下料开夹夹具固定气缸23(Q:R2305)上限(I:R1310)报警！', 'A', 'D15240.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm330', '下料开夹夹具固定气缸23(Q:R2305)下限(I:R1310)报警！', 'A', 'D15240.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm331', '两侧精整A缓存夹具固定气缸24(Q:R2306)上限(I:R1311)报警！', 'A', 'D15240.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm332', '两侧精整A缓夹具固定气缸24(Q:R2306)下限(I:R1311)报警！', 'A', 'D15240.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm333', '前转运夹具固定气缸25(Q:R2307)上限(I:R1312)报警！', 'A', 'D15240.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm334', '前转运夹具固定气缸25(Q:R2307)下限(I:R1312)报警！', 'A', 'D15240.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm335', '等待温度中异常报警！', 'A', 'D15240.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm336', '备用63015', 'A', 'D15240.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm337', '备用63100', 'A', 'D15241.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm338', '备用63101', 'A', 'D15241.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm339', '备用63102', 'A', 'D15241.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm340', '备用63103', 'A', 'D15241.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm341', '备用63104', 'A', 'D15241.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm342', '备用63105', 'A', 'D15241.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm343', '备用63106', 'A', 'D15241.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm344', '备用63107', 'A', 'D15241.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm345', '备用63108', 'A', 'D15241.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm346', '备用63109', 'A', 'D15241.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm347', '备用63110', 'A', 'D15241.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm348', '备用63111', 'A', 'D15241.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm349', '备用63112', 'A', 'D15241.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm350', '备用63113', 'A', 'D15241.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm351', '备用63114', 'A', 'D15241.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm352', '备用63115', 'A', 'D15241.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm353', 'A-1上料翻转伺服电机报警或轴错误中！', 'A', 'D15242.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm354', 'A-2裙边整形伺服电机报警或轴错误中！', 'A', 'D15242.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm355', 'A-3上料搬远平移伺服电机报警或轴错误中！', 'A', 'D15242.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm356', 'A-4上料搬远升降伺服电机报警或轴错误中！', 'A', 'D15242.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm357', 'A-5上料推电芯伺服电机报警或洲轴误中！', 'A', 'D15242.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm358', 'A-6上料锁紧伺服电机报警或轴错误中！', 'A', 'D15242.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm359', 'A-7上料定位伺服电机报警或轴错误中！', 'A', 'D15242.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm360', 'A-8左折角伺服电机报警或轴错误中！', 'A', 'D15242.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm361', 'B-9中间拨快1伺服电机报警或轴错误中！', 'A', 'D15242.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm362', 'B-10中间拨快2伺服电机报警或轴错误中！', 'A', 'D15242.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm363', 'A-11中间拨快3伺服电机报警或轴错误中', 'A', 'D15242.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm364', 'A-12中间拨快4伺服电机报警或轴错误中！', 'A', 'D15242.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm365', '备用63212', 'A', 'D15242.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm366', 'A-14换向锁紧伺服报警或轴错误中！', 'A', 'D15242.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm367', 'A-15换向定位伺服电机报警或轴错误中！', 'A', 'D15242.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm368', 'A-16后转运伺服电机报警或轴错误中！', 'A', 'D15242.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm369', 'B-1右折角伺服电机报警或轴错误中！', 'A', 'D15243.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm370', 'B-2右1折前后伺服电机报警或轴错误中！', 'A', 'D15243.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm371', 'B-3右1折边伺服电机报警或轴错误中！', 'A', 'D15243.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm372', 'B-4中间拨快6伺服电机报警或轴错误中！', 'A', 'D15243.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm373', 'B-5中间拨快7伺服电机报警或轴错误中！', 'A', 'D15243.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm374', '备用63305', 'A', 'D15243.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm375', '备用63306', 'A', 'D15243.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm376', '备用63307', 'A', 'D15243.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm377', 'B-9中间拨快8伺服电机报警或轴错误中！', 'A', 'D15243.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm378', 'B-10下料开夹伺服电机报警或轴错误中！', 'A', 'D15243.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm379', 'B-11推电芯伺服电机报警或轴错误中！', 'A', 'D15243.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm380', 'B-12下料搬运平移伺服电机报警或轴错误中！', 'A', 'D15243.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm381', 'B-13下料升降伺服电机报警或轴错误中！', 'A', 'D15243.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm382', 'B-14前转运伺服电机报警或轴错误中！', 'A', 'D15243.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm383', 'B-15下料翻转平移伺服电机报警或轴错误中！', 'A', 'D15243.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm384', 'B-16下料翻转升降伺服电机报警或轴错误中！', 'A', 'D15243.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm385', '备用63400', 'A', 'D15244.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm386', '备用63401', 'A', 'D15244.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm387', '备用63402', 'A', 'D15244.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm388', '备用63403', 'A', 'D15244.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm389', '备用63404', 'A', 'D15244.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm390', '备用63405', 'A', 'D15244.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm391', '备用63406', 'A', 'D15244.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm392', '备用63407', 'A', 'D15244.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm393', '备用63408', 'A', 'D15244.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm394', '备用63409', 'A', 'D15244.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm395', '备用63410', 'A', 'D15244.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm396', '备用63411', 'A', 'D15244.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm397', '备用63412', 'A', 'D15244.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm398', '备用63413', 'A', 'D15244.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm399', '备用63414', 'A', 'D15244.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm400', '备用63415', 'A', 'D15244.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm401', '备用63500', 'A', 'D15245.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm402', '备用63501', 'A', 'D15245.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm403', '备用63502', 'A', 'D15245.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm404', '备用63503', 'A', 'D15245.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm405', '备用63504', 'A', 'D15245.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm406', '备用63505', 'A', 'D15245.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm407', '备用63506', 'A', 'D15245.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm408', '备用63507', 'A', 'D15245.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm409', '备用63508', 'A', 'D15245.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm410', '备用63509', 'A', 'D15245.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm411', '备用63510', 'A', 'D15245.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm412', '备用63511', 'A', 'D15245.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm413', '备用63512', 'A', 'D15245.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm414', '备用63513', 'A', 'D15245.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm415', '备用63514', 'A', 'D15245.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm416', '备用63515', 'A', 'D15245.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm417', '备用63600', 'A', 'D15246.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm418', '备用63601', 'A', 'D15246.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm419', '备用63602', 'A', 'D15246.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm420', '备用63603', 'A', 'D15246.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm421', '备用63604', 'A', 'D15246.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm422', '备用63605', 'A', 'D15246.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm423', '备用63606', 'A', 'D15246.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm424', '备用63607', 'A', 'D15246.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm425', '备用63608', 'A', 'D15246.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm426', '备用63609', 'A', 'D15246.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm427', '备用63610', 'A', 'D15246.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm428', '备用63611', 'A', 'D15246.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm429', '备用63612', 'A', 'D15246.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm430', '备用63613', 'A', 'D15246.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm431', '备用63614', 'A', 'D15246.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm432', '备用63615', 'A', 'D15246.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm433', '备用63700', 'A', 'D15247.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm434', '备用63701', 'A', 'D15247.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm435', '备用63702', 'A', 'D15247.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm436', '备用63703', 'A', 'D15247.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm437', '备用63704', 'A', 'D15247.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm438', '备用63705', 'A', 'D15247.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm439', '备用63706', 'A', 'D15247.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm440', '备用63707', 'A', 'D15247.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm441', '备用63708', 'A', 'D15247.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm442', '备用63709', 'A', 'D15247.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm443', '备用63710', 'A', 'D15247.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm444', '备用63711', 'A', 'D15247.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm445', '备用63712', 'A', 'D15247.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm446', '备用63713', 'A', 'D15247.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm447', '备用63714', 'A', 'D15247.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm448', '备用63715', 'A', 'D15247.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm449', '备用63800', 'A', 'D15248.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm450', '备用63801', 'A', 'D15248.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm451', '备用63802', 'A', 'D15248.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm452', '备用63803', 'A', 'D15248.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm453', '备用63804', 'A', 'D15248.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm454', '备用63805', 'A', 'D15248.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm455', '备用63806', 'A', 'D15248.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm456', '备用63807', 'A', 'D15248.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm457', '备用63808', 'A', 'D15248.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm458', '备用63809', 'A', 'D15248.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm459', '备用63810', 'A', 'D15248.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm460', '备用63811', 'A', 'D15248.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm461', '备用63812', 'A', 'D15248.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm462', '备用63813', 'A', 'D15248.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm463', '备用63814', 'A', 'D15248.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm464', '备用63815', 'A', 'D15248.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm465', '24#两侧精整A缓存位夹具感应异常，请摆正夹具！', 'A', 'D15249.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm466', '1#上料锁紧位夹具感应异常，请摆正夹具！', 'A', 'D15249.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm467', '25#前转运位未感应到夹具，请摆正夹具！', 'A', 'D15249.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm468', '备用63903', 'A', 'D15249.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm469', '25#前转运位感应到夹具，请摆正夹具！', 'A', 'D15249.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm470', '12#换向锁紧位夹具感应异常，请摆正夹具！', 'A', 'D15249.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm471', '14#右切边位夹具感应异常，请摆正夹具！', 'A', 'D15249.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm472', '13#后转运位未感应到夹具，请摆正夹具！', 'A', 'D15249.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm473', '13#后转运位感应到夹具，请摆正夹具！', 'A', 'D15249.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm474', '总气压低（R0011），请检查压缩空气开关是否打开！', 'A', 'D15249.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm475', 'ANDON系统未打开，请检查！', 'A', 'D15249.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm476', '下料翻转有电池，请手动取走并手动关闭真空！', 'A', 'D15249.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm477', '未收到MFG结果,请检查MFG设置！', 'A', 'D15249.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm478', '扫码异常，请检查！', 'A', 'D15249.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm479', '扫码软件未打开，请检查扫码位置或MFG！', 'A', 'D15249.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm480', '备用63915', 'A', 'D15249.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm481', '左滴胶压力未达到（R0615），请检查！', 'A', 'D15250.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm482', '右滴胶压力未达到（R0715），请检查！', 'A', 'D15250.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm483', '报警页面弹出超过一分钟未选择处理类型！', 'A', 'D15250.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm484', '下料搬运A有电池，请手动取走并手动关闭真空！', 'A', 'D15250.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm485', '下料搬运B有电池，请手动取走并手动关闭真空！', 'A', 'D15250.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm486', '下料搬运C有电池，请手动取走并手动关闭真空！', 'A', 'D15250.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm487', '备用64006', 'A', 'D15250.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm488', '超设定最高温度报警，请检查！', 'A', 'D15250.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm489', '备用64008', 'A', 'D15250.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm490', '备用64009', 'A', 'D15250.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm491', '左废料盒已满(R0711),请清理废料！', 'A', 'D15250.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm492', '右废料盒已满(R1313),请清理废料！', 'A', 'D15250.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm493', '备用64012', 'A', 'D15250.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm494', '备用64013', 'A', 'D15250.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm495', '备用64014', 'A', 'D15250.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm496', '备用64015', 'A', 'D15250.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm497', '备用64100', 'A', 'D15251.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm498', '备用64101', 'A', 'D15251.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm499', '备用64102', 'A', 'D15251.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm500', '备用64103', 'A', 'D15251.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm501', '备用64104', 'A', 'D15251.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm502', '备用64105', 'A', 'D15251.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm503', '备用64106', 'A', 'D15251.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm504', '备用64107', 'A', 'D15251.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm505', '备用64108', 'A', 'D15251.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm506', '备用64109', 'A', 'D15251.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm507', '备用64110', 'A', 'D15251.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm508', '备用64111', 'A', 'D15251.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm509', '备用64112', 'A', 'D15251.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm510', '备用64113', 'A', 'D15251.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm511', '备用64114', 'A', 'D15251.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm512', '备用64115', 'A', 'D15251.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm513', 'NG料盒已满，有F098/F099风险，请取走电芯再开机！', 'A', 'D15252.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm514', '1#上料锁紧位夹具感应异常，请摆正夹具！', 'A', 'D15252.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm515', '2#左切边位夹具感应异常，请摆正夹具！', 'A', 'D15252.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm516', '3#左折角位夹具感应异常，请摆正夹具！', 'A', 'D15252.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm517', '4#左1折热压位夹具感应异常，请摆正夹具！', 'A', 'D15252.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm518', '5#滴胶位夹具感应异常，请摆正夹具！', 'A', 'D15252.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm519', '6#左精整位夹具感应异常，请摆正夹具！', 'A', 'D15252.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm520', '7#左2折热烫A位夹具感应异常，请摆正夹具！', 'A', 'D15252.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm521', '8#左2折热烫B位夹具感应异常，请摆正夹具！', 'A', 'D15252.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm522', '9#左2折冷烫A位夹具感应异常，请摆正夹具！', 'A', 'D15252.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm523', '10#左2折冷烫B位夹具感应异常，请摆正夹具！', 'A', 'D15252.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm524', '11#左水平精整位夹具感应异常，请摆正夹具！', 'A', 'D15252.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm525', '12#换向锁紧位夹具感应异常，请摆正夹具！', 'A', 'D15252.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm526', '13#后转运位夹具感应异常，请摆正夹具！', 'A', 'D15252.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm527', '14#右切边位夹具感应异常，请摆正夹具！', 'A', 'D15252.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm528', '15#右折角位夹具感应异常，请摆正夹具！', 'A', 'D15252.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm529', '16#右1折边位夹具感应异常，请摆正夹具！', 'A', 'D15253.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm530', '17#右1折热压位夹具感应异常，请摆正夹具！', 'A', 'D15253.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm531', '18#右滴胶位夹具感应异常，请摆正夹具！', 'A', 'D15253.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm532', '19#右精整位夹具感应异常，请摆正夹具！', 'A', 'D15253.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm533', '20#右2折热烫A位夹具感应异常，请摆正夹具！', 'A', 'D15253.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm534', '21#右2折热烫B位夹具感应异常，请摆正夹具！', 'A', 'D15253.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm535', '22#右2折冷烫A位夹具感应异常，请摆正夹具！', 'A', 'D15253.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm536', '23#下料开夹位夹具感应异常，请摆正夹具！', 'A', 'D15253.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm537', '24#两侧精整A缓存位夹具感应异常，请摆正夹具！', 'A', 'D15253.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm538', '25#前转运位夹具感应异常，请摆正夹具！', 'A', 'D15253.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm539', '左1折热压位感应电芯异常，请手动取出电芯，并清除记忆！', 'A', 'D15253.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm540', '右1折热压位感应电芯异常，请手动取出电芯，并清除记忆！', 'A', 'D15253.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm541', '13#后转运位夹具感应异常，请摆正夹具！', 'A', 'D15253.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm542', '25#前转运位夹具感应异常，请摆正夹具！', 'A', 'D15253.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm543', '扫码气缸（R2315）缩回位（R714）报警，请检查!', 'A', 'D15253.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm544', '扫码气缸（R2315）伸出位（R1314）报警，请检查!', 'A', 'D15253.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm545', '裙边整形<左>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm546', 'MFG界面信息不全（例如员工工号为空）,请检查！', 'A', 'D15254.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm547', 'MFG系统关闭,请检查！', 'A', 'D15254.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm548', '裙边整形<右>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm549', '连续3个扫码不良,请检查！', 'A', 'D15254.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm550', '拦截二次返修电芯,请挑出不良电芯！', 'A', 'D15254.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm551', '左一折热压<上>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm552', '左一折热压<下>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm553', 'BarCode于物料编码MI不匹配！', 'A', 'D15254.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm554', 'FEF热熔胶批次已过期！', 'A', 'D15254.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm555', '左二折热烫<A>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm556', '左二折热烫<B>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm557', '右一折热压<上>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm558', '右一折热压<下>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm559', '右二折热烫<A>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15254.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm560', '换向锁紧位夹具未打开，请检查（R0713）!', 'A', 'D15254.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm561', '左2折热烫气缸A两感应器缩回(R309)、伸出(R310)同时亮报警，请检查！', 'A', 'D15255.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm562', '左2折热烫气缸B两感应器缩回(R312)、伸出(R313)同时亮报警，请检查！', 'A', 'D15255.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm563', '左2折冷烫气缸A两感应器缩回(R315)、伸出(R400)同时亮报警，请检查！', 'A', 'D15255.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm564', '左2折冷烫气缸B两感应器缩回(R401)、伸出(R402)同时亮报警，请检查！', 'A', 'D15255.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm565', '右2折热烫气缸A两感应器缩回(R907)、伸出(R908)同时亮报警，请检查！', 'A', 'D15255.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm566', '右2折热烫气缸B两感应器缩回(R910)、伸出(R911)同时亮报警，请检查！', 'A', 'D15255.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm567', '右2折冷烫气缸A两感应器缩回(R913)、伸出(R914)同时亮报警，请检查！', 'A', 'D15255.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm568', '两侧精整A<左>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15255.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm569', '两侧精整A<右>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15255.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm570', '清洁各工位时间到报警提示，请打开安全门清洁各工位后才能解除报警！', 'A', 'D15255.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm571', 'MFG软件未打开或MFG界面信息不全报警,请检查MFG软件！', 'A', 'D15255.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm572', '左滴胶机温度未达到设定值报警(R713),请等待！', 'A', 'D15255.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm573', '右滴胶机温度未达到设定值报警(R1003),请等待！', 'A', 'D15255.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm574', '两侧精整B<左>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15255.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm575', '两侧精整B<右>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15255.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm576', '右二折热烫<B>设定温度不在上、下限范围内报警，请重新输入！', 'A', 'D15255.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm577', '上料夹具开合防呆感应异常（711），请检查上料锁紧位夹具是否开启！', 'A', 'D15256.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm578', '滴胶停机超时报警，请手动排胶处理！（左滴胶R1715）', 'A', 'D15256.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm579', '滴胶停机超时报警，请手动排胶处理！（右滴胶R2010）', 'A', 'D15256.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm580', '裙边整形左加热超高温报警，请检查！！！', 'A', 'D15256.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm581', '裙边整形右加热超高温报警，请检查！！！', 'A', 'D15256.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm582', '左一折热压上加热超高温报警，请检查！！！', 'A', 'D15256.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm583', '左一折热压下加热超高温报警，请检查！！！', 'A', 'D15256.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm584', '左二折热烫A加热超高温报警，请检查！！！', 'A', 'D15256.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm585', '左二折热烫B加热超高温报警，请检查！！！', 'A', 'D15256.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm586', '右一折热压上加热超高温报警，请检查！！！', 'A', 'D15256.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm587', '右一折热压下加热超高温报警，请检查！！！', 'A', 'D15256.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm588', '右二折热烫A加热超高温报警，请检查！！！', 'A', 'D15256.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm589', '右二折热烫B加热超高温报警，请检查！！！', 'A', 'D15256.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm590', '两侧精整A左加热超高温报警，请检查！！！', 'A', 'D15256.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm591', '两侧精整A右加热超高温报警，请检查！！！', 'A', 'D15256.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm592', '两侧精整B左加热超高温报警，请检查！！！', 'A', 'D15256.15', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm593', '两侧精整B右加热超高温报警，请检查！！！', 'A', 'D15257.0', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm594', '下料开夹锁紧气缸不在缩回位，请检查感应器R1003是否异常或者气', 'A', 'D15257.1', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm595', '下料开夹锁紧气缸不在伸出位，请检测气缸是否堵塞。', 'A', 'D15257.2', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm596', '备用64703', 'A', 'D15257.3', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm597', '备用64704', 'A', 'D15257.4', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm598', '换向夹具锁紧气缸不在缩回位报警，请检查！！！', 'A', 'D15257.5', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm599', '换向夹具锁紧气缸不在伸出位报警，请检查！！！', 'A', 'D15257.6', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm600', '下料开夹具锁紧气缸不在縮回位报警，请检查！！！', 'A', 'D15257.7', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm601', '下料开夹具锁紧气缸不在伸出位报警，请检查！！！', 'A', 'D15257.8', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm602', '进料防呆报警（上料TAB未感应到）', 'A', 'D15257.9', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm603', '(首件模式)请取走右冷烫B电芯！', 'A', 'D15257.10', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm604', '备用64711', 'A', 'D15257.11', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm605', '备用64712', 'A', 'D15257.12', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm606', '备用64713', 'A', 'D15257.13', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm607', '备用64714', 'A', 'D15257.14', '2020-08-18 17:29:09'),
	('ACOA0022', 1, 'Alarm608', '备用64715', 'A', 'D15257.15', '2020-08-18 17:29:09');
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

-- Dumping data for table ptf.device_controlcode: ~25 rows (大约)
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
	(1, 'ACOA0022', 1, '001', '50819', '左滴胶枪温度(℃)', 'Float', 'D17184', 'D17186', 'D17188', 'D15000', 'D17184', 'D15784', 'D15028', 1),
	(2, 'ACOA0022', 1, '001', '50820', '右滴胶枪温度(℃)', 'Float', 'D17190', 'D17192', 'D17194', 'D15000', '', 'D15790', 'D15030', 1),
	(3, 'ACOA0022', 1, '130513', '50950', '左一折热压上烫头温度(℃)', 'Float', 'D17112', 'D17114', 'D17116', 'D15000', '', 'D15712', 'D15004', 1),
	(4, 'ACOA0022', 1, '130514', '50951', '左一折热压下烫头温度(℃)', 'Float', 'D17118', 'D17120', 'D17122', 'D15000', '', 'D15718', 'D15006', 1),
	(5, 'ACOA0022', 1, '130515', '50952', '右一折热压上烫头温度(℃)', 'Float', 'D17136', 'D17138', 'D17140', 'D15000', '', 'D15736', 'D15012', 1),
	(6, 'ACOA0022', 1, '130516', '50953', '右一折热压下烫头温度(℃)', 'Float', 'D17142', 'D17144', 'D17146', 'D15000', '', 'D15742', 'D15014', 1),
	(7, 'ACOA0022', 1, '130517', '50954', '左二折热烫A温度(℃)', 'Float', 'D17124', 'D17126', 'D17128', 'D15000', '', 'D15724', 'D15008', 1),
	(8, 'ACOA0022', 1, '130518', '50955', '左二折热烫B温度(℃)', 'Float', 'D17130', 'D17132', 'D17134', 'D15000', '', 'D15730', 'D15010', 1),
	(9, 'ACOA0022', 1, '130519', '50956', '右二折热烫A温度(℃)', 'Float', 'D17148', 'D17150', 'D17152', 'D15000', '', 'D15748', 'D15016', 1),
	(10, 'ACOA0022', 1, '130520', '50957', '右二折热烫B温度(℃)', 'Float', 'D17154', 'D17156', 'D17158', 'D15000', '', 'D15754', 'D15018', 1),
	(11, 'ACOA0022', 1, '130521', '50821', '裙边整形左温度(℃)', 'Float', 'D17100', 'D17102', 'D17104', 'D15000', '', 'D15700', 'D15000', 1),
	(12, 'ACOA0022', 1, '130522', '50822', '裙边整形右温度(℃)', 'Float', 'D17106', 'D17108', 'D17110', 'D15000', '', 'D15706', 'D15002', 1),
	(13, 'ACOA0022', 1, '130523', '50958', '精烫A左温度(℃)', 'Float', 'D17160', 'D17162', 'D17164', 'D15000', '', 'D15760', 'D15020', 1),
	(14, 'ACOA0022', 1, '130524', '50959', '精烫A右温度(℃)', 'Float', 'D17166', 'D17168', 'D17170', 'D15000', '', 'D15766', 'D15022', 1),
	(15, 'ACOA0022', 1, '130525', '50960', '精烫B左温度(℃)', 'Float', 'D17172', 'D17174', 'D17176', 'D15000', '', 'D15772', 'D15024', 1),
	(16, 'ACOA0022', 1, '130526', '50961', '精烫B右温度(℃)', 'Float', 'D17178', 'D17180', 'D17182', 'D15000', '', 'D15778', 'D15026', 1),
	(17, 'ACOA0022', 1, '130527', '50962', '左一折热压气压(MPa)', 'Float', 'D17280', 'D17282', 'D17284', 'D15000', '', 'D15870', 'D15032', 1),
	(18, 'ACOA0022', 1, '130528', '50963', '右一折热压气压(MPa)', 'Float', 'D17286', 'D17288', 'D17290', 'D15000', '', 'D15872', 'D15034', 1),
	(19, 'ACOA0022', 1, '130529', '50964', '左二折热烫A气压(MPa)', 'Float', 'D17292', 'D17294', 'D17296', 'D15000', '', 'D15874', 'D15036', 1),
	(20, 'ACOA0022', 1, '130530', '50965', '左二折热烫B气压(MPa)', 'Float', 'D17298', 'D17300', 'D17302', 'D15000', '', 'D15876', 'D15038', 1),
	(21, 'ACOA0022', 1, '130531', '50966', '右二折热烫A气压(MPa)', 'Float', 'D17304', 'D17306', 'D17308', 'D15000', '', 'D15878', 'D15040', 1),
	(22, 'ACOA0022', 1, '130532', '50967', '右二折热烫B气压(MPa)', 'Float', 'D17310', 'D17312', 'D17314', 'D15000', '', 'D15880', 'D15042', 1),
	(23, 'ACOA0022', 1, '130533', '50968', '左二折冷烫A气压(MPa)', 'Float', 'D17316', 'D17318', 'D17320', 'D15000', '', 'D15882', 'D15044', 1),
	(24, 'ACOA0022', 1, '130534', '50969', '左二折冷烫B气压(MPa)', 'Float', 'D17322', 'D17324', 'D17326', 'D15000', '', 'D15884', 'D15046', 1),
	(25, 'ACOA0022', 1, '130535', '50970', '右二折冷烫A气压(MPa)', 'Float', 'D17328', 'D17330', 'D17332', 'D15000', '', 'D15886', 'D15048', 1),
	(26, 'ACOA0022', 1, '130536', '50971', '右二折冷烫B气压(MPa)', 'Float', 'D17334', 'D17336', 'D17338', 'D15000', '', 'D15888', 'D15050', 1),
	(27, 'ACOA0022', 1, '130537', '50972', '左三合一精整水平平推气压', 'Float', 'D17340', 'D17342', 'D17344', 'D15000', '', 'D15890', 'D15052', 1),
	(28, 'ACOA0022', 1, '130538', '50973', '右三合一精整水平平推气压', 'Float', 'D17346', 'D17348', 'D17350', 'D15000', '', 'D15892', 'D15054', 1),
	(29, 'ACOA0022', 1, '130539', '50974', '精烫A气压(MPa)', 'Float', 'D17352', 'D17354', 'D17356', 'D15000', '', 'D15894', 'D15056', 1),
	(30, 'ACOA0022', 1, '130540', '50975', '精烫B气压(MPa)', 'Float', 'D17358', 'D17360', 'D17362', 'D15000', '', 'D15896', 'D15058', 1),
	(31, 'ACOA0022', 1, '130541', '99999', '左一折热压时间(s)', 'Float', 'D17202', 'D17204', 'D17206', 'D15000', '', 'D15802', 'D15080', 1),
	(32, 'ACOA0022', 1, '130542', '50976', '左二折热烫1时间(s)', 'Float', 'D17208', 'D17210', 'D17212', 'D15000', '', 'D15808', 'D15082', 1),
	(33, 'ACOA0022', 1, '130543', '50977', '左二折热烫2时间(s)', 'Float', 'D17214', 'D17216', 'D17218', 'D15000', '', 'D15814', 'D15084', 1),
	(34, 'ACOA0022', 1, '130544', '50978', '左二折冷烫1时间(s)', 'Float', 'D17220', 'D17222', 'D17224', 'D15000', '', 'D15820', 'D15086', 1),
	(35, 'ACOA0022', 1, '130545', '50979', '左二折冷烫2时间(s)', 'Float', 'D17226', 'D17228', 'D17230', 'D15000', '', 'D15826', 'D15088', 1),
	(36, 'ACOA0022', 1, '130546', '50980', '右一折热压时间(s)', 'Float', 'D17232', 'D17234', 'D17236', 'D15000', '', 'D15832', 'D15090', 1),
	(37, 'ACOA0022', 1, '130547', '50981', '右二折热烫1时间(s)', 'Float', 'D17238', 'D17240', 'D17242', 'D15000', '', 'D15838', 'D15092', 1),
	(38, 'ACOA0022', 1, '130548', '50982', '右二折热烫2时间(s)', 'Float', 'D17244', 'D17246', 'D17248', 'D15000', '', 'D15844', 'D15094', 1),
	(39, 'ACOA0022', 1, '130549', '50983', '右二折冷烫1时间(s)', 'Float', 'D17250', 'D17252', 'D17254', 'D15000', '', 'D15850', 'D15096', 1),
	(40, 'ACOA0022', 1, '130550', '50984', '右二折冷烫2时间(s)', 'Float', 'D17256', 'D17258', 'D17260', 'D15000', '', 'D15856', 'D15098', 1),
	(41, 'ACOA0022', 1, '130551', '50985', '裙边整形时间(s)', 'Float', 'D17262', 'D17264', 'D17266', 'D15000', '', 'D15862', 'D15100', 1),
	(42, 'ACOA0022', 1, '130552', '16248', '冷烫温度(℃)', 'Float', 'D17268', 'D17270', 'D17272', 'D15000', '', 'D15868', 'D15102', 1),
	(43, 'ACOA0022', 1, '130553', '50986', '左通道滴胶机气压(MPa)', 'Float', 'D17364', 'D17366', 'D17368', 'D15000', '', 'D15898', 'D15084', 1),
	(44, 'ACOA0022', 1, '130554', '50987', '右通道滴胶机气压(MPa)', 'Float', 'D17370', 'D17372', 'D17374', 'D15000', '', 'D15900', 'D15086', 1);
/*!40000 ALTER TABLE `device_inputoutput_config` ENABLE KEYS */;

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
	(1, 'ACOA0022', 1, 'temp1', '关键配件预期寿命1', 'Float', 100, 0, 0, 'test1', 'test1', 'test2', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(2, 'ACOA0022', 1, 'temp2', '关键配件预期寿命2', 'Float', 100, 0, 0, 'D17054', 'D17054', 'D15452', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(3, 'ACOA0022', 1, 'temp3', '关键配件预期寿命3', 'Float', 100, 0, 0, 'D17056', 'D17056', 'D15454', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(4, 'ACOA0022', 1, 'temp4', '关键配件预期寿命4', 'Float', 100, 0, 0, 'D17058', 'D17058', 'D15456', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(5, 'ACOA0022', 1, 'temp5', '关键配件预期寿命5', 'Float', 100, 0, 0, 'D17060', 'D17060', 'D15458', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(6, 'ACOA0022', 1, 'temp6', '关键配件预期寿命6', 'Float', 100, 0, 0, 'D17062', 'D17062', 'D15460', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(7, 'ACOA0022', 1, 'temp7', '关键配件预期寿命7', 'Float', 100, 0, 0, 'D17064', 'D17064', 'D15462', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(8, 'ACOA0022', 1, 'temp8', '关键配件预期寿命8', 'Float', 100, 0, 0, 'D17066', 'D17066', 'D15464', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(9, 'ACOA0022', 1, 'temp9', '关键配件预期寿命9', 'Float', 100, 0, 0, 'D17068', 'D17068', 'D15466', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin'),
	(10, 'ACOA0022', 1, 'temp10', '关键配件预期寿命10', 'Float', 100, 0, 0, 'D17070', 'D17070', 'D15468', '', 1, 0.9500000, '2019-11-04 16:14:10', 'SuperAdmin');
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='用户自定义变量里重要参数记录，数据有变化的时候才记录';

-- Dumping data for table ptf.hmi_setting_monitor_log: ~0 rows (大约)
/*!40000 ALTER TABLE `hmi_setting_monitor_log` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=530 DEFAULT CHARSET=utf8 COMMENT='input参数下发历史数据以及手动上传PLC的历史数据\r\nEQID为设备名称：Baking设备必填字段，因为每个炉子可能做不一样的Model,每个炉子下发的时间点不一样。\r\nDataTime表示从PLC上传的时间或者下发到PLC的时间\r\nmodel字段表示当前生产的型号\r\ninput_variableDID表示input_variable表里的主键ID\r\n则需要在user_fefine_variable表里建立20个控制温度变量，个对应1个炉子，每个控制温度变量绑定一个PLC。';

-- Dumping data for table ptf.input_variable_history: ~365 rows (大约)
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
) ENGINE=InnoDB AUTO_INCREMENT=2567 DEFAULT CHARSET=utf8 COMMENT='软件运行日志\r\nloglevel=INFO日志是会在在UI界面上显示查看的\r\nloglevel=DEBUG日志只是在txt格式的log文件里查看到\r\nloglevel=ERROR表示代码逻辑异常的日志\r\nloglevel=WARN表示警告提醒的日志信息\r\nloglevel=FATAl表示软件try catch到的异常信息';

-- Dumping data for table ptf.log4net: ~64 rows (大约)
/*!40000 ALTER TABLE `log4net` DISABLE KEYS */;
INSERT INTO `log4net` (`id`, `logdate`, `loglevel`, `logger`, `message`, `exception`, `softwareName`) VALUES
	(2554, '2020-08-18 17:29:08.197', 'INFO', '', '软件启动', '', 'PTF'),
	(2555, '2020-08-18 17:29:10.287', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(2556, '2020-08-18 17:29:10.371', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(2557, '2020-08-18 17:29:10.535', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(2558, '2020-08-18 17:29:11.622', 'INFO', '', '初始化连接PLC[127.0.0.1]failure：错误代号:10000\r\n文本描述:由于目标计算机积极拒绝，无法连接。 127.0.0.1:5000', '', 'PTF'),
	(2559, '2020-08-18 17:29:13.632', 'INFO', '', '初始化连接PLC[127.0.0.1]failure：错误代号:10000\r\n文本描述:由于目标计算机积极拒绝，无法连接。 127.0.0.1:5000', '', 'PTF'),
	(2560, '2020-08-18 17:29:14.295', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(2561, '2020-08-18 17:29:14.426', 'INFO', '', '08-18 17:29:14 设备ID:ACOA0022注册成功', '', 'PTF'),
	(2562, '2020-08-18 17:29:15.631', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(2563, '2020-08-18 17:29:15.646', 'INFO', '', '初始化连接PLC[127.0.0.1]failure：错误代号:10000\r\n文本描述:由于目标计算机积极拒绝，无法连接。 127.0.0.1:5000', '', 'PTF'),
	(2564, '2020-08-18 17:29:15.652', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF'),
	(2565, '2020-08-18 17:29:17.655', 'INFO', '', '初始化连接PLC[127.0.0.1]failure：错误代号:10000\r\n文本描述:由于目标计算机积极拒绝，无法连接。 127.0.0.1:5000', '', 'PTF'),
	(2566, '2020-08-18 17:29:19.664', 'INFO', '', '初始化连接PLC[127.0.0.1]failure：错误代号:10000\r\n文本描述:由于目标计算机积极拒绝，无法连接。 127.0.0.1:5000', '', 'PTF');
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
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COMMENT='操作记录信息/参数变更记录信息\r\n比如操作了某个运行按钮，操作了停止按钮，需要记录在此表\r\n比如软件记录MES ID的值发生了变化，则也需要将MES ID变化前的值和变化后的值记录在此表';

-- Dumping data for table ptf.log_operation: ~8 rows (大约)
/*!40000 ALTER TABLE `log_operation` DISABLE KEYS */;
INSERT INTO `log_operation` (`id`, `datatime`, `username`, `Action`) VALUES
	(15, '2020-08-18 17:29:15.653', 'superAdmin', 'MES登陆');
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
) ENGINE=InnoDB AUTO_INCREMENT=1337 DEFAULT CHARSET=utf8 COMMENT='变量自动监控记录表';

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
) ENGINE=InnoDB AUTO_INCREMENT=4924 DEFAULT CHARSET=utf8 COMMENT='MES接口上传数据表，包括了上传MES的信息和MES返回的信息\r\nResoponseTime:上位机上传数据给MES后，MES返回信息的时间，单位为ms';

-- Dumping data for table ptf.log_simple_mes_interface_execution: ~78 rows (大约)
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` DISABLE KEYS */;
INSERT INTO `log_simple_mes_interface_execution` (`id`, `datatime`, `FunctionID`, `GUID`, `ResponseTime`, `RequestTime`, `Data`, `errorMsg`) VALUES
	(4920, '2020-08-18 17:29:14.289', 'A001', 'daf06b79-3baa-43f7-a685-3a1e89438835', NULL, '2020-08-18 17:29:14.283', '{"Header":{"SessionID":"daf06b79-3baa-43f7-a685-3a1e89438835","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:29:14 283"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0022"}}', ''),
	(4921, '2020-08-18 17:29:14.294', 'A002', 'daf06b79-3baa-43f7-a685-3a1e89438835', '2020-08-18 17:29:14.293', '2020-08-18 17:29:14.293', '{"Header":{"SessionID":"daf06b79-3baa-43f7-a685-3a1e89438835","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:29:14 283","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:29:14 288"},"ResponseInfo":{"Result": "OK"}}', ''),
	(4922, '2020-08-18 17:29:15.628', 'A039', '2126f712-b493-424d-9047-3ca46de76881', NULL, '2020-08-18 17:29:15.624', '{"Header":{"SessionID":"2126f712-b493-424d-9047-3ca46de76881","FunctionID":"A039","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:29:15 624","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(4923, '2020-08-18 17:29:15.632', 'A040', '2126f712-b493-424d-9047-3ca46de76881', '2020-08-18 17:29:15.631', '2020-08-18 17:29:15.631', '{"Header":{"SessionID":"2126f712-b493-424d-9047-3ca46de76881","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:29:15 624","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:29:15 628"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', '');
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

-- Dumping data for table ptf.plc_area_config: ~8 rows (大约)
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
	(1, 'PLC1', 'FEF', '127.0.0.1', '5000', 'MC_3E_Binary', NULL, 1);
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

-- Dumping data for table ptf.plc_protocol: ~19 rows (大约)
/*!40000 ALTER TABLE `plc_protocol` DISABLE KEYS */;
INSERT INTO `plc_protocol` (`ProtocolName`, `model`, `remark`) VALUES
	('ADS', 'Beckhoff', '上位机只读写一个结构体，结构体的成员必须与BeckhoffTwinCAT命名空间里RWstruct结构体里的成员一致'),
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
	(4, '设备报警数据', 1, 'D', '15220', 120, 'R', 1000, '2019-09-19 16:50:00', 1),
	(14, 'input监控地址', 1, 'D', '17100', 100, 'R', 1000, '2019-10-21 09:46:00', 1),
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

-- Dumping data for table ptf.roles: ~0 rows (大约)
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` (`roleId`, `roleName`, `MesUserLevel`, `UserLevelPLCValue`, `permissionCodes`, `createTime`, `modifyTime`, `remark`) VALUES
	(0, 'Development Authority', 'Administrator', 408, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:29:09', '超级管理员,禁止删除'),
	(1, 'Maintain Authority', 'Maintainer', 307, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:29:09', '系统管理员'),
	(2, 'Operator Authority', 'Operator', 206, 'UserManager,UserManager.Personal,UserManager.User', '2019-07-09 09:50:00', '2020-08-18 17:29:09', 'ME人员'),
	(10, 'Guest Authority', 'Guest', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,MES,MES.MESweb,Maintain,DataWareHouse,SystemSetting,LogQuery,UserManager,UserManager.Login,DeviceControl', '2019-07-02 11:04:39', '2020-08-18 17:29:09', '尚未登陆');
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

-- Dumping data for table ptf.server_backpara: ~0 rows (大约)
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

-- Dumping data for table ptf.users: ~0 rows (大约)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`userId`, `userName`, `password`, `name`, `roleId`, `createTime`, `gender`, `LastLoginTime`, `remark`) VALUES
	(1, 'admin', 'admin', 'admin', 1, '2017-03-23 21:05:21', 1, '2019-07-09 08:20:38', NULL),
	(2, 'Guest', 'Guest', 'Guest', 10, '2017-03-23 21:05:21', 1, '2020-08-18 17:29:08', NULL),
	(3, 'chen', 'chen', 'Michal', 2, '0001-01-01 00:00:00', 0, NULL, NULL),
	(4, 'SuperAdmin', 'SuperAdmin', 'SuperAdmin', 0, '2019-07-08 13:22:06', 0, '2019-12-13 16:07:00', NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=119 DEFAULT CHARSET=utf8 COMMENT='用户定义变量表。variableTypeID等于1表示为系统变量，系统变量与PLC变量，不是在同一个UI界面上显示的。\r\n系统变量不会与PLC地址绑定。非系统变量可能会与PLC地址绑定关联，用于接口数据上传。\r\n每个MES ID的变量关联的plc_rw_area_did都必须是Ｒ读类型的．MES ID的变量属于OUTPUT参数，上传给mes的．needMonitorLog都必须为１\r\nmes input参数变量也全部needMonitorLog必须是1';

-- Dumping data for table ptf.user_define_variable: ~0 rows (大约)
/*!40000 ALTER TABLE `user_define_variable` DISABLE KEYS */;
INSERT INTO `user_define_variable` (`variableID`, `variableName`, `variableTypeID`, `value`, `description`, `valueTypes`, `variableLength`, `plc_rw_area_did`, `plc_address`, `needMonitorLog`, `remark`, `datatime`) VALUES
	(8, 'ClearHmiSettingMonitorLog', 1, '30', '重点监控参数保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:56:24'),
	(9, 'ClearMESinterfaceLog', 1, '3', 'MES接口调用记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:29:09'),
	(10, 'ClearAlarmRecord', 1, '31', '历史报警记录保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:51:01'),
	(11, 'ClearPLCinteractiveLog', 1, '31', 'PC/PLC交互记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:51:01'),
	(12, 'dayShiftStartTime', 1, '7:30', '白班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(13, 'nightShiftStartTime', 1, '19:31', '夜班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(14, 'AssetsNO', 1, 'ACOA0022', '设备编号', 'String', 1, NULL, NULL, b'0', '不可删除此', '2019-10-19 14:20:09'),
	(27, 'Model', 1, 'M20', '生产型号', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:50'),
	(28, 'PLCversion', 2, '3.87.895', 'PLC程序版本号', 'String', 3, 21, 'D4', b'0', NULL, '2020-02-22 17:05:22'),
	(32, 'CCDVersion', 1, '3.02.6', 'CCD软件版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-09-24 19:14:34'),
	(33, 'DefaultMesUrl', 1, 'https://www.baidu.com/', '默认MES网址', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:55'),
	(46, 'ControlCode', 2, '0', 'mes控机操作的PLC地址', 'Int16', 1, 21, 'D17030', b'0', NULL, '2020-02-22 16:56:21'),
	(47, 'MESstatusToPLC', 2, '10', 'MES连接状态toPLC', 'Int32', 1, 21, 'D17002', b'0', NULL, '2020-02-22 16:56:30'),
	(48, 'HeatBeat', 2, '10', '心跳', 'Float', 1, 21, 'D17006', b'0', NULL, '2020-02-22 16:56:32'),
	(49, 'ParentEQState', 2, '0', '设备状态', 'Int16', 1, 21, 'D15200', b'0', NULL, '2020-02-22 16:56:33'),
	(50, 'AndonState', 2, '0', 'Andon状态', 'Int16', 1, 21, 'D15201', b'0', NULL, '2020-02-22 17:16:11'),
	(51, 'Quantity', 2, '0', '当班产量', 'Int32', 1, 21, 'D15202', b'0', NULL, '2020-02-22 16:56:38'),
	(52, 'HmiPermissionRequest', 2, '0', 'PLC获取HMI权限触发', 'Int16', 1, 21, 'D9', b'0', NULL, '2020-02-22 16:56:42'),
	(53, 'Account', 2, '0', 'PLC账户', 'String', 6, 21, 'D10', b'0', NULL, '2020-02-22 16:56:43'),
	(54, 'Code', 2, '0', 'PLC账户密码', 'String', 6, 21, 'D13', b'0', NULL, '2020-02-22 16:56:46'),
	(55, 'UserLevel', 2, '0', 'PLC账户等级', 'Int16', 1, 21, 'D17004', b'0', NULL, '2020-02-22 16:56:47'),
	(65, 'localIpAdr', 1, '127.0.0.1', '本机 IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:24:52'),
	(67, 'localUdpSendPortNo', 1, '61000', '本机udp发送端口', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-01-06 13:44:43'),
	(68, 'localUdpRecvPortNo', 1, '50002', '本机udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2020-01-06 13:44:41'),
	(69, 'localTcpPortNo', 1, '60001', '本机tcp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-09-27 08:50:09'),
	(70, 'serverIpAdr', 1, '127.0.0.1', 'MES服务端IP地址', 'String', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:25:07'),
	(71, 'serverUdpPortNo', 1, '50000', 'MES服务端udp接收端口号', 'Int32', 1, NULL, NULL, b'1', NULL, '2019-09-23 16:27:54'),
	(72, 'HMIversion', 2, '1.25.3544', 'HMI版本', 'String', 3, 21, 'D11', b'1', NULL, '2020-02-22 17:02:12'),
	(91, 'MesReply', 2, NULL, 'MES响应状态', 'Int32', 1, 21, 'D17190', b'0', NULL, '2020-02-22 17:15:46'),
	(92, 'test1', 2, '100', '预期寿命', 'Float', 1, 21, 'D0', b'0', NULL, '2020-02-22 17:00:04'),
	(93, 'test2', 2, '60', '当前寿命', 'Float', 1, 21, 'D2', b'0', NULL, '2020-02-22 17:04:33'),
	(94, 'ClearLog4net', 1, '30', '软件运行日志保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:45'),
	(95, 'ClearOperationLog', 1, '30', '操作日志保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:47'),
	(96, 'ClearInputDownloadHistory', 1, '180', 'Input下发历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-22 08:22:41'),
	(97, 'ClearProductionData', 1, '30', '生产记录数据保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:54:50'),
	(98, 'ClearInputUploadHistory', 1, '60', 'Output上载历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-22 08:37:05'),
	(99, 'A007Count', 2, '0', 'A007首件数量', 'Int16', 1, 21, 'D15204', b'0', NULL, '2020-08-18 17:29:09'),
	(100, 'StateCode', 2, '0', 'A007首件信号', 'Int16', 1, 21, 'D15205', b'0', NULL, '2020-08-18 17:29:09'),
	(101, 'LabChinese', 1, 'FEF', '工序名称(中文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-01-06 13:43:52'),
	(102, 'LabEnglish', 1, 'FEF', '工序名称(英文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-01-06 13:43:52'),
	(103, 'LabVersion', 1, 'FEF', '版本号', 'String', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:29:09'),
	(117, 'AutoPopMonitorPage', 1, '1', '是否启用电脑无操作几分钟后自动弹出监控画面', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:29:09'),
	(118, 'DefaultAcount', 1, 'superAdmin', 'Default Acount', 'String', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:29:09');
/*!40000 ALTER TABLE `user_define_variable` ENABLE KEYS */;

-- Dumping structure for table ptf.valuetypes
CREATE TABLE IF NOT EXISTS `valuetypes` (
  `valueType` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  PRIMARY KEY (`valueType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='变量类型';

-- Dumping data for table ptf.valuetypes: ~0 rows (大约)
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

-- Dumping data for table ptf.variabletype: ~0 rows (大约)
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
