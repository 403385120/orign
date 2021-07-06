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
  `level_name` varchar(50) NOT NULL,
  `remark` varchar(50) NOT NULL,
  UNIQUE KEY `level_name` (`level_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报警等级';

-- Dumping data for table ptf.alarm_level: ~4 rows (大约)
/*!40000 ALTER TABLE `alarm_level` DISABLE KEYS */;
INSERT INTO `alarm_level` (`level_name`, `remark`) VALUES
	('A', '需要ME工程师处理'),
	('B', '需要现场操作员处理'),
	('C', '设备动作过程中的联锁及提示类信息'),
	('D', '备用');
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报警记录';

-- Dumping data for table ptf.alarm_record: ~0 rows (大约)
/*!40000 ALTER TABLE `alarm_record` DISABLE KEYS */;
/*!40000 ALTER TABLE `alarm_record` ENABLE KEYS */;

-- Dumping structure for event ptf.alarm_record_cutclip
DELIMITER //
CREATE DEFINER=`root`@`localhost` EVENT `alarm_record_cutclip` ON SCHEDULE EVERY 3 SECOND STARTS '2019-07-23 14:13:09' ENDS '2029-07-23 14:13:09' ON COMPLETION NOT PRESERVE ENABLE COMMENT '将报警信息从临时表剪切放入记录表' DO BEGIN
	delete from alarm_id_for_cut where 1=1;
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='dispose_state = 0 表示暂未处理的报警\r\ndispose_state = 1 表示暂已处理的报警\r\nduration表示报警时长(min)';

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
	('ACOA0023', 1, '{\r\n	"Header": {\r\n		"SessionID": "20282823-D4FA-48F5-A297-387EE7B2EBD5",\r\n		"FunctionID": "A007",\r\n		"PCName": "NMRD-I92150-N",\r\n		"EQCode": "ACOA0023",\r\n		"SoftName": "ServerSoft",\r\n		"RequestTime": "2019-12-26 09:18:09 772"\r\n	},\r\n	"RequestInfo": {\r\n		"Count": "5",\r\n		"CmdInfo": {\r\n			"ControlCode": "RUN",\r\n			"StateCode": "1",\r\n			"StateDesc": "123"\r\n		},\r\n		"UserInfo": {\r\n			"UserID": "123",\r\n			"UserName": "ATL",\r\n			"UserLevel": "1"\r\n		},\r\n		"ResourceInfo": {\r\n			"ResourceID": "EQ00000001",\r\n			"ResourceShift": "M"\r\n		},\r\n		"SpartInfo": [\r\n			{\r\n				"SpartName": "正涂胶辊寿命",\r\n				"SpartID": "BLCX-ZTJG-S",\r\n				"SpartExpectedLifetime": "200",\r\n				"UsedLife": "10",\r\n				"ChangeFlag": "true"\r\n\r\n			},\r\n			{\r\n				"SpartName": "反涂胶辊寿命",\r\n				"SpartID": "BLCX-FTJG-S",\r\n				"SpartExpectedLifetime": "300",\r\n				"UsedLife": "20",\r\n				"ChangeFlag": "true"\r\n			}\r\n		],\r\n		"ModelInfo": "123",\r\n		"ParameterInfo": [\r\n			{\r\n				"ParamID": "47812",\r\n				"StandardValue": "45",\r\n				"UpperLimitValue": "50",\r\n				"LowerLimitValue": "30",\r\n				"Description": "生产速度"\r\n			},\r\n			{\r\n				"ParamID": "47813",\r\n				"StandardValue": "46",\r\n				"UpperLimitValue": "50",\r\n				"LowerLimitValue": "30",\r\n				"Description": "收卷气压"\r\n			},\r\n			{\r\n				"ParamID": "47814",\r\n				"StandardValue": "8",\r\n				"UpperLimitValue": "50",\r\n				"LowerLimitValue": "30",\r\n				"Description": "放卷气压"\r\n			}\r\n		]\r\n	}\r\n}', 'ControlCode', 'ParentEQState', 'AndonState', 'Quantity', 'MESstatusToPLC', 'HeatBeat', 'MesReply', 'HmiPermissionRequest', 'Account', 'Code', 'UserLevel', 'A007Count', 'StateCode', NULL, NULL, NULL, NULL);
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
  KEY `FK_device_alert_config_alarm_level` (`AlertLevel`),
  CONSTRAINT `FK_device_alert_config_alarm_level` FOREIGN KEY (`AlertLevel`) REFERENCES `alarm_level` (`level_name`),
  CONSTRAINT `FK_device_alert_config_deivce_equipmentid_plc` FOREIGN KEY (`EquipmentID`) REFERENCES `deivce_equipmentid_plc` (`EquipmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='设备报警配置\r\nisAlarming字段适用于板卡设备，报警信息并非从PLC中获取得到。对于板卡设备，AlertBitAddr字段无意义。';

-- Dumping data for table ptf.device_alert_config: ~128 rows (大约)
/*!40000 ALTER TABLE `device_alert_config` DISABLE KEYS */;
INSERT INTO `device_alert_config` (`EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `AlertLevel`, `AlertBitAddr`, `DataTime`) VALUES
	('ACOA0023', 1, 'Alarm001', '有轴报警,请查看代码，并复位开机', 'B', 'DB1000.302.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm002', '气压保护输出，请检查', 'B', 'DB1000.302.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm003', '错位偏差过大，请修正', 'B', 'DB1000.302.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm004', '放卷压板合，不能开机', 'C', 'DB1000.302.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm005', '放卷压板合，不能开机', 'C', 'DB1000.302.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm006', '收卷压板合，不能开机', 'C', 'DB1000.302.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm007', '正涂电眼没有激活，请检查或者重新上电', 'B', 'DB1000.302.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm008', '反涂电眼没有激活，请检查或者重新上电', 'B', 'DB1000.302.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm017', '加热关闭，不能开机', 'C', 'DB1000.303.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm018', '正涂横向电眼未到测量范围', 'B', 'DB1000.303.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm019', '定长停机距离到达，请检查', 'C', 'DB1000.303.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm020', '套色系统正涂检测信号丢失', 'C', 'DB1000.303.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm021', '套色系统反涂检测信号丢失', 'C', 'DB1000.303.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm022', '测长系统检测差报警', 'C', 'DB1000.303.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm023', '测长系统正涂检测信号丢失', 'C', 'DB1000.303.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm024', '测长系统反涂检测信号丢失', 'C', 'DB1000.303.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm033', '放卷急停按下请检查', 'B', 'DB1000.304.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm034', '正涂急停按下请检查', 'B', 'DB1000.304.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm035', '反涂急停按下请检查', 'B', 'DB1000.304.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm036', '收卷急停按下请检查', 'B', 'DB1000.304.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm037', '放卷张力调节PID没有激活，请注意', 'C', 'DB1000.304.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm038', '收卷张力调节PID没有激活，请注意', 'C', 'DB1000.304.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm039', '入牵张力调节PID没有激活，请注意', 'C', 'DB1000.304.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm040', '出牵张力调节PID没有激活，请注意', 'C', 'DB1000.304.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm049', '收卷报警卷经到达', 'C', 'DB1000.305.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm050', '收卷停机卷经到达', 'C', 'DB1000.305.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm051', '放卷报警卷经到达', 'C', 'DB1000.305.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm052', '放卷停机卷经到达', 'C', 'DB1000.305.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm053', '回零错误，请停机后，手动回零，再开机', 'B', 'DB1000.305.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm054', '回零错误，请停机后，手动回零，再开机', 'B', 'DB1000.305.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm055', '外部检测系统测长超差，请检查', 'B', 'DB1000.305.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm056', '横向套位超差，请注意', 'B', 'DB1000.305.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm065', '横向移动位置超差，左右移动值不能大于10，请重新输入左右极限值', 'B', 'DB1000.306.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm066', '位置反向，请检查，右限制值要大于左限位值', 'B', 'DB1000.306.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm067', '正涂烘箱超过机械监视温度，请检查', 'A', 'DB1000.306.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm068', '反涂烘箱超过机械监视温度，请检查', 'A', 'DB1000.306.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm069', '正涂超过机械监视温度，请检查', 'A', 'DB1000.306.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm070', '反涂超过机械监视温度，请检查', 'A', 'DB1000.306.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm071', '主排风变频报警，请检查', 'A', 'DB1000.306.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm072', '正涂烘箱变频报警，请检查', 'A', 'DB1000.306.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm081', '反涂烘箱变频报警，请检查', 'A', 'DB1000.307.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm082', '正涂变频报警，请检查', 'A', 'DB1000.307.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm083', '反涂变频报警，请检查', 'A', 'DB1000.307.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm084', '正涂压辊限位到达，请检查', 'B', 'DB1000.307.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm085', '反涂压辊限位到达，请检查', 'B', 'DB1000.307.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm086', '平推电机向前保护10秒，禁止向前', 'B', 'DB1000.307.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm087', '平推电机超出限位，请检查', 'A', 'DB1000.307.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm088', '平推电机超出限位，请检查', 'A', 'DB1000.307.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm097', '正涂电机故障，请检查故障代码', 'A', 'DB1000.308.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm098', '反涂电机故障，请检查故障代码', 'A', 'DB1000.308.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm099', '入料牵引电机故障，请检查故障代码', 'A', 'DB1000.308.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm100', '出料牵引电机故障，请检查故障代码', 'A', 'DB1000.308.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm101', '正涂烘箱电机故障，请检查故障代码', 'A', 'DB1000.308.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm102', '反涂烘箱电机故障，请检查故障代码', 'A', 'DB1000.308.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm103', '正涂压辊电机故障，请检查故障代码', 'A', 'DB1000.308.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm104', '反涂压辊电机故障，请检查故障代码', 'A', 'DB1000.308.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm113', '放卷电机故障，请检查故障代码', 'A', 'DB1000.309.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm114', '收卷电机故障，请检查故障代码', 'A', 'DB1000.309.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm115', '设备正在维修，请等待', 'A', 'DB1000.309.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm116', 'No value', 'D', 'DB1000.309.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm117', '设备维修中，不能开机', 'A', 'DB1000.309.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm118', 'No value', 'D', 'DB1000.309.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm119', '平推电机进限位到达，请检查', 'B', 'DB1000.309.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm120', '平推电机退限位到达，请检查', 'B', 'DB1000.309.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm129', '电柜风扇1报警，请检查', 'A', 'DB1000.310.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm130', '电柜风扇2报警，请检查', 'A', 'DB1000.310.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm131', '电柜风扇3报警，请检查', 'A', 'DB1000.310.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm132', '电柜风扇4报警，请检查', 'A', 'DB1000.310.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm133', '电柜风扇5报警，请检查', 'A', 'DB1000.310.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm134', '电柜风扇6报警，请检查', 'A', 'DB1000.310.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm135', '烘箱3超温报警，请检查', 'B', 'DB1000.310.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm136', '烘箱4超温报警，请检查', 'B', 'DB1000.310.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm145', '维修测长到达，请检修设备并清零。', 'B', 'DB1000.311.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm146', '维修计米1到达，请检修设备并清零', 'B', 'DB1000.311.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm147', '维修计米2到达，请检修设备并清零', 'B', 'DB1000.311.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm148', '维修计米3到达，请检修设备并清零', 'B', 'DB1000.311.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm149', '维修计米4到达，请检修设备并清零', 'B', 'DB1000.311.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm150', 'No value', 'D', 'DB1000.311.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm151', 'No value', 'D', 'DB1000.311.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm152', '反涂横向电眼未到测量范围', 'B', 'DB1000.311.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm153', '收卷功能块输出错误，请按下复位按钮复位', 'B', 'DB1000.312.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm154', '放卷功能块输出错误，请按下复位按钮复位', 'B', 'DB1000.312.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm155', '入牵报警，请查看代码，并复位', 'A', 'DB1000.312.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm156', '出牵报警，请查看代码，并复位', 'A', 'DB1000.312.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm157', '有轴未同步，请检查', 'B', 'DB1000.312.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm158', '收放卷条件不满足，不能启动，请关闭收放卷电源，再启动', 'B', 'DB1000.312.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm159', '正涂没有匀墨，请手动操作', 'B', 'DB1000.312.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm160', '反涂没有匀墨，请手动操作', 'B', 'DB1000.312.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm161', '正涂压辊位置左右偏差超过3mm,请调平或者回零操作！', 'B', 'DB1000.313.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm162', '正涂零位操作，请执行回零操作', 'B', 'DB1000.313.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm163', '反涂压辊位置左右偏差超过3mm,请调平或者回零操作！', 'B', 'DB1000.313.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm164', '反涂零位操作，请执行回零操作', 'B', 'DB1000.313.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm165', 'Andon系统处于离线模式请，注意！', 'C', 'DB1000.313.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm166', 'Andon系统订单未完成，禁止开机！', 'C', 'DB1000.313.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm167', 'Andon系统生命信号缺失，请注意！', 'C', 'DB1000.313.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm168', 'No value', 'D', 'DB1000.313.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm169', '电柜门已开启，请检查', 'A', 'DB1000.314.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm170', '反涂风机变频报警，请检查', 'A', 'DB1000.314.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm171', '烘箱1风机变频报警，请检查', 'A', 'DB1000.314.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm172', '烘箱2风机变频报警，请检查', 'A', 'DB1000.314.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm173', '烘箱3风机变频报警，请检查', 'A', 'DB1000.314.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm174', '烘箱4风机变频报警，请检查', 'A', 'DB1000.314.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm175', '正涂版辊没有ReadyToPowerUp，请关掉空带后，点击复位按钮', 'B', 'DB1000.314.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm176', '反涂版辊没有ReadyToPowerUp，请关掉空带后，点击复位按钮', 'B', 'DB1000.314.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm177', '正涂版辊齿轮比计算不正常,请等待15秒,或切换版间隙数,或重新上电', 'B', 'DB1000.315.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm178', '反涂版辊齿轮比计算不正常,请等待15秒,或切换版间隙数,或重新上电', 'B', 'DB1000.315.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm179', '接近辊过于接近收卷', 'B', 'DB1000.315.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm180', '正涂电眼移动电机软限位到达', 'B', 'DB1000.315.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm181', '反涂电眼移动电机软限位到达', 'B', 'DB1000.315.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm182', '正涂套色电机软限位到达', 'B', 'DB1000.315.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm183', '反涂套色电机软限位到达', 'B', 'DB1000.315.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm184', '横向错位偏差过大，请注意！', 'C', 'DB1000.315.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm185', '正涂刮刀未合，不能压辊！不能开机！', 'C', 'DB1000.316.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm186', '反涂刮刀未合，不能压辊！不能开机！', 'C', 'DB1000.316.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm187', '正涂刮刀过载，请注意！', 'C', 'DB1000.316.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm188', '反涂刮刀过载，请注意！', 'C', 'DB1000.316.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm189', '正涂横向电机驱动侧硬限位到达！', 'B', 'DB1000.316.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm190', '正涂横向电机操作侧硬限位到达！', 'B', 'DB1000.316.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm191', '反涂横向电机驱动侧硬限位到达！', 'B', 'DB1000.316.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm192', '反涂横向电机操作侧硬限位到达！', 'B', 'DB1000.316.7', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm193', '485通讯故障！请注意！', 'A', 'DB1000.317.0', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm194', '电柜超温！请注意！', 'A', 'DB1000.317.1', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm195', '加热关闭，不能开机！', 'B', 'DB1000.317.2', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm196', 'No value', 'D', 'DB1000.317.3', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm197', 'No value', 'D', 'DB1000.317.4', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm198', 'No value', 'D', 'DB1000.317.5', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm199', 'No value', 'D', 'DB1000.317.6', '2020-08-18 17:21:52'),
	('ACOA0023', 1, 'Alarm200', 'No value', 'D', 'DB1000.317.7', '2020-08-18 17:21:52');
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
	('HeatBeat', '', '写入PLC的心跳值', 10),
	('HOLD_ENG(Suspend)', 'HOLD', '由MES 下达指令给到设备，设备暂停加工，不允许新的物料进入设备中加工', 8),
	('HOLD_ENG(Suspend)', 'WAIT_ENG', '生产部发现设备有问题，将设备交由工程师处理', 9),
	('IDLE', 'IDLE', '设备空闲', 3),
	('IDLE', 'Switch_Shift', '设备正常，由于需要换班或午休', 5),
	('IDLE', 'WAIT_Material', '设备正常，因为无料生产等候', 4),
	('IDLE', 'WAIT_PRD', '设备工程师修完机之后交付给生产部', 6),
	('InputLimitControl', 'SET', 'Input总控，下发Input成功后输出总控开关值', 111),
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
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COMMENT='从A060接口接收到的Input参数和Output参数地址配置信息\r\n\r\nEnableAddr对应的自定义变量用于表示是否启用该Input参数，该自定义变量的值等于10表示不启用，等于0表示启用。此表可能配置了100个参数，\r\n但是A007接口可能只下发了几个。A007下发的那几个参数对应的EnableAddr等于0，其他的等于10.\r\n\r\n当同一个参数的input参数ID和output参数ID不同时上传接收A007指令时使用input参数ID进行接收，上传A013、A017数据时使用output参数ID进行上传；\r\n当同一个参数的input参数ID和output参数ID不同时监控input参数设定值变化A045的上传指令使用output参数ID进行上传；\r\n\r\ndownloadStatus等于1表示下载完成.\r\nMES下发model后软件需要将downloadStatus字段值改为0.\r\nvariableTypeID与variableType表里的variableTypeID对应，默认为0，表示刚从MES下载此配置，暂未判断addr为PLC地址，还是系统变量名称';

-- Dumping data for table ptf.device_inputoutput_config: ~13 rows (大约)
/*!40000 ALTER TABLE `device_inputoutput_config` DISABLE KEYS */;
INSERT INTO `device_inputoutput_config` (`ID`, `EquipmentID`, `plcID`, `SendParamID`, `UploadParamID`, `ParamName`, `Type`, `SettingValueAddr`, `UpperLimitValueAddr`, `LowerLimitValueAddr`, `LimitControl`, `InputChangeMonitorAddr`, `ActualValueAddr`, `BycellOutputAddr`, `ParamValueRatio`) VALUES
	(1, 'ACOA0023', 1, '47812', '47812', '生产速度', 'float', 'DB2000.142', 'DB2000.146', 'DB2000.150', 'DB1000.204', 'DB1000.1226', 'DB1000.1022', '', 1),
	(2, 'ACOA0023', 1, '47813', '47813', '收卷气压', 'float', 'DB2000.154', 'DB2000.158', 'DB2000.162', 'DB1000.206', 'DB1000.1230', 'DB1000.1026', '', 1),
	(3, 'ACOA0023', 1, '47814', '47814', '放卷气压', 'float', 'DB2000.166', 'DB2000.170', 'DB2000.174', 'DB1000.208', 'DB1000.1234', 'DB1000.1030', '', 1),
	(4, 'ACOA0023', 1, 'S1', 'U1', '放卷张力', 'float', 'DB2000.178', 'DB2000.182', 'DB2000.186', 'DB1000.210', 'DB1000.1238', 'DB1000.1034', '', 1),
	(5, 'ACOA0023', 1, '47815', '47815', '入牵张力', 'float', 'DB2000.178', 'DB2000.182', 'DB2000.186', 'DB1000.212', 'DB1000.1242', 'DB1000.1038', '', 1),
	(6, 'ACOA0023', 1, '47816', '47816', '出牵张力', 'float', 'DB2000.202', 'DB2000.206', 'DB2000.210', 'DB1000.214', 'DB1000.1246', 'DB1000.1042', '', 1),
	(7, 'ACOA0023', 1, 'S2', 'U2', '收卷张力', 'float', 'DB2000.214', 'DB2000.218', 'DB2000.222', 'DB1000.216', 'DB1000.1250', 'DB1000.1046', '', 1),
	(8, 'ACOA0023', 1, '47817', '50942', '烘箱一温度', 'float', 'DB2000.226', 'DB2000.230', 'DB2000.234', 'DB1000.218', 'DB1000.1254', 'DB1000.1050', '', 1),
	(9, 'ACOA0023', 1, '47817', '50943', '烘箱二温度', 'float', 'DB2000.238', 'DB2000.242', 'DB2000.246', 'DB1000.220', 'DB1000.1258', 'DB1000.1054', '', 1),
	(10, 'ACOA0023', 1, '47818', '50944', '烘箱一风频', 'float', 'DB2000.250', 'DB2000.254', 'DB2000.258', 'DB1000.222', 'DB1000.1262', 'DB1000.1058', '', 1),
	(11, 'ACOA0023', 1, '47818', '50945', '烘箱二风频', 'float', 'DB2000.262', 'DB2000.266', 'DB2000.270', 'DB1000.224', 'DB1000.1266', 'DB1000.1062', '', 1),
	(12, 'ACOA0023', 1, '51781', '51781', '排风一风频', 'float', 'DB2000.274', 'DB2000.278', 'DB2000.282', 'DB1000.226', 'DB1000.1270', 'DB1000.1066', '', 1),
	(13, 'ACOA0023', 1, 'S13', 'U13', '排风二风频', 'float', 'DB2000.286', 'DB2000.290', 'DB2000.294', 'DB1000.228', 'DB1000.1274', 'DB1000.1070', '', 1);
/*!40000 ALTER TABLE `device_inputoutput_config` ENABLE KEYS */;

-- Dumping structure for table ptf.device_spart_config
CREATE TABLE IF NOT EXISTS `device_spart_config` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentID` varchar(50) NOT NULL COMMENT '设备ID',
  `plcID` int(11) DEFAULT NULL,
  `UploadParamID` varchar(50) NOT NULL COMMENT '关键配件ID',
  `ParamName` varchar(50) NOT NULL COMMENT '关键配件名称',
  `Type` varchar(20) NOT NULL COMMENT '参数类型 Float,Int32',
  `SpartExpectedLifetime` int(11) NOT NULL COMMENT '关键配件预期寿命(A007设置到软件)',
  `MesSettingUsedLife` float NOT NULL COMMENT 'MES下发当前使用寿命',
  `NeedDownLoadSpartExpectedLifetimeToPLC` int(11) NOT NULL,
  `SettingValueAddr` varchar(50) NOT NULL COMMENT '关键配件预期寿命(A007/A060设置到软件，A007配置到Need之后写入PLC)',
  `SettingActualValueAddr` varchar(50) NOT NULL COMMENT 'MES下发的实际使用寿命地址',
  `ActualValueAddr` varchar(50) NOT NULL COMMENT '关键配件实际使用寿命(A021读取,A007/A060设置到软件)',
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
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8 COMMENT='易损件配置表\r\n在更好零部件的时候，需要将本表的数据（被更换的零部件的数据）剪切放入历史表quickwearparthistoryinfo\r\nSettingValueAddr为PLC地址或者系统变量名\r\nActualValueAddr为PLC地址或者系统变量名\r\nSpartExpectedLifetime为MES A007接口下发的值，作为本地保存，不会自动保存到PLC中。需要额外下发到PLC或者系统变量\r\nNeedDownLoadSpartExpectedLifetimeToPLC = 1表示需要下载到PLC\r\n在PLC通信正常的情况下，在一个单独的线程里，预期寿命下载完成后，上位机对ActualValueAddr地址里的值清零。将自动将NeedDownLoadSpartExpectedLifetimeToPLC改为0';

-- Dumping data for table ptf.device_spart_config: ~10 rows (大约)
/*!40000 ALTER TABLE `device_spart_config` DISABLE KEYS */;
INSERT INTO `device_spart_config` (`id`, `EquipmentID`, `plcID`, `UploadParamID`, `ParamName`, `Type`, `SpartExpectedLifetime`, `MesSettingUsedLife`, `NeedDownLoadSpartExpectedLifetimeToPLC`, `SettingValueAddr`, `SettingActualValueAddr`, `ActualValueAddr`, `LimitControl`, `ParamValueRatio`, `PercentWarning`, `ChangeDate`, `ChangeUser`) VALUES
	(1, 'ACOA0023', 1, 'BLCX-ZTJG-S', '正涂胶辊寿命', 'Float', 200, 0, 0, 'DB2000.406', 'DB2000.406', 'DB1000.362', 'DB2000.700.0', 1, 0.9500000, '2019-12-27 14:32:41', 'ATL'),
	(2, 'ACOA0023', 1, 'BLCX-FTJG-S', '反涂胶辊寿命', 'Float', 300, 0, 0, 'DB2000.410', 'DB2000.410', 'DB1000.366', 'DB2000.700.1', 1, 0.9500000, '2019-12-27 14:32:41', 'ATL'),
	(3, 'ACOA0023', 1, 'BLCX-ZZPC-S', '正涂左版堵寿命', 'Float', 100, 0, 0, 'DB2000.414', 'DB2000.414', 'DB1000.370', 'DB2000.700.2', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(4, 'ACOA0023', 1, 'BLCX-ZYPC-S', '正涂右版堵寿命', 'Float', 100, 0, 0, 'DB2000.418', 'DB2000.418', 'DB1000.374', 'DB2000.700.3', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(5, 'ACOA0023', 1, 'BLCX-FZPC-S', '反涂左版堵寿命', 'Float', 100, 0, 0, 'DB2000.422', 'DB2000.422', 'DB1000.378', 'DB2000.700.4', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(6, 'ACOA0023', 1, 'BLCX-FYPC-S', '反涂右版堵寿命', 'Float', 100, 0, 0, 'DB2000.426', 'DB2000.426', 'DB1000.382', 'DB2000.700.5', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(7, 'ACOA0023', 1, 'BLCX-RQJG-S', '入牵胶辊寿命', 'Float', 100, 0, 0, 'DB2000.430', 'DB2000.430', 'DB1000.386', 'DB2000.700.6', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(8, 'ACOA0023', 1, 'BLCX-CQJG-S', '出牵胶辊寿命', 'Float', 100, 0, 0, 'DB2000.434', 'DB2000.434', 'DB1000.390', 'DB2000.700.7', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(9, 'ACOA0023', 1, 'BLCX-ZTBG-S', '正涂版辊寿命', 'Float', 100, 0, 0, 'DB2000.438', 'DB2000.438', 'DB1000.394', 'DB2000.701.0', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin'),
	(10, 'ACOA0023', 1, 'BLCX-FTBG-S', '反涂版辊寿命', 'Float', 100, 0, 0, 'DB2000.442', 'DB2000.442', 'DB1000.398', '', 1, 0.9500000, '2019-11-04 16:14:00', 'SuperAdmin');
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
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8 COMMENT='用户自定义变量里重要参数记录，数据有变化的时候才记录';

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
) ENGINE=InnoDB AUTO_INCREMENT=610 DEFAULT CHARSET=utf8 COMMENT='input参数下发历史数据以及手动上传PLC的历史数据\r\nEQID为设备名称：Baking设备必填字段，因为每个炉子可能做不一样的Model,每个炉子下发的时间点不一样。\r\nDataTime表示从PLC上传的时间或者下发到PLC的时间\r\nmodel字段表示当前生产的型号\r\ninput_variableDID表示input_variable表里的主键ID\r\n则需要在user_fefine_variable表里建立20个控制温度变量，个对应1个炉子，每个控制温度变量绑定一个PLC。';

-- Dumping data for table ptf.input_variable_history: ~74 rows (大约)
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
) ENGINE=InnoDB AUTO_INCREMENT=67 DEFAULT CHARSET=utf8 COMMENT='软件运行日志\r\nloglevel=INFO日志是会在在UI界面上显示查看的\r\nloglevel=DEBUG日志只是在txt格式的log文件里查看到\r\nloglevel=ERROR表示代码逻辑异常的日志\r\nloglevel=WARN表示警告提醒的日志信息\r\nloglevel=FATAl表示软件try catch到的异常信息';

-- Dumping data for table ptf.log4net: ~38 rows (大约)
/*!40000 ALTER TABLE `log4net` DISABLE KEYS */;
INSERT INTO `log4net` (`id`, `logdate`, `loglevel`, `logger`, `message`, `exception`, `softwareName`) VALUES
	(55, '2020-08-18 17:21:20.045', 'INFO', '', 'MES返回的errmsg：A024---- 00   ', '', 'PTF'),
	(56, '2020-08-18 17:21:20.050', 'INFO', '', '上传版本号成功', '', 'PTF'),
	(57, '2020-08-18 17:21:20.056', 'INFO', '', 'MES返回的errmsg：A020---- 00   ', '', 'PTF'),
	(58, '2020-08-18 17:21:20.069', 'INFO', '', 'A019发送设备状态 ACOA0022', '', 'PTF'),
	(59, '2020-08-18 17:21:50.974', 'INFO', '', '软件启动', '', 'PTF'),
	(60, '2020-08-18 17:21:52.990', 'Error', '', '反序列化[lstA047RequestFromFile]failure：System.IO.FileNotFoundException: 未能找到文件“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”。\r\n文件名:“E:MTW20200807PTF UIDebuglstA047RequestFromMES.dat”\r\n   在 System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\r\n   在 System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)\r\n   在 System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)\r\n   在 System.IO.File.OpenRead(String path)\r\n   在 ATL.Common.WRSerializable.DeserializeFromFile[T](T& _description, String _filePath) 位置 E:MTW20200807ATL.CommonWRSerializable.cs:行号 24\r\n   在 ATL.MES.InterfaceClient.doWork(Object state) 位置 E:MTW20200807ATL.MESInterfaceClient.cs:行号 587', '', 'PTF'),
	(61, '2020-08-18 17:21:53.066', 'INFO', '', 'Connect MES主服务器 Success!', '', 'PTF'),
	(62, '2020-08-18 17:21:53.176', 'INFO', '', '系统配置OK，已启动上位机通信', '', 'PTF'),
	(63, '2020-08-18 17:21:56.965', 'INFO', '', 'MES返回的errmsg：A002---- 00   ', '', 'PTF'),
	(64, '2020-08-18 17:21:57.070', 'INFO', '', '08-18 17:21:57 设备ID:ACOA0023注册成功', '', 'PTF'),
	(65, '2020-08-18 17:21:58.002', 'INFO', '', 'MES返回的errmsg：A040---- 00   ', '', 'PTF'),
	(66, '2020-08-18 17:21:58.012', 'INFO', '', '账号:superAdmin 登陆MES成功，权限为:Administrator', '', 'PTF');
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
) ENGINE=InnoDB AUTO_INCREMENT=108 DEFAULT CHARSET=utf8 COMMENT='操作记录信息/参数变更记录信息\r\n比如操作了某个运行按钮，操作了停止按钮，需要记录在此表\r\n比如软件记录MES ID的值发生了变化，则也需要将MES ID变化前的值和变化后的值记录在此表';

-- Dumping data for table ptf.log_operation: ~3 rows (大约)
/*!40000 ALTER TABLE `log_operation` DISABLE KEYS */;
INSERT INTO `log_operation` (`id`, `datatime`, `username`, `Action`) VALUES
	(107, '2020-08-18 17:21:58.013', 'superAdmin', 'MES登陆');
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
) ENGINE=InnoDB AUTO_INCREMENT=463 DEFAULT CHARSET=utf8 COMMENT='变量自动监控记录表';

-- Dumping data for table ptf.log_plc_interactive: ~12 rows (大约)
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
) ENGINE=InnoDB AUTO_INCREMENT=919 DEFAULT CHARSET=utf8 COMMENT='MES接口上传数据表，包括了上传MES的信息和MES返回的信息\r\nResoponseTime:上位机上传数据给MES后，MES返回信息的时间，单位为ms';

-- Dumping data for table ptf.log_simple_mes_interface_execution: ~30 rows (大约)
/*!40000 ALTER TABLE `log_simple_mes_interface_execution` DISABLE KEYS */;
INSERT INTO `log_simple_mes_interface_execution` (`id`, `datatime`, `FunctionID`, `GUID`, `ResponseTime`, `RequestTime`, `Data`, `errorMsg`) VALUES
	(911, '2020-08-18 17:21:20.042', 'A023', 'e065b603-b8e1-4e6a-a0d4-f129b95ff520', NULL, '2020-08-18 17:21:20.041', '{"Header":{"SessionID":"e065b603-b8e1-4e6a-a0d4-f129b95ff520","FunctionID":"A023","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:21:20 041"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"ACOA0022","ProgramName":"上位机","ProgramVension":"FEF"},{"ResourceID":"ACOA0022","ProgramName":"PLC","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"HMI","ProgramVension":"123"},{"ResourceID":"ACOA0022","ProgramName":"CCDVersion","ProgramVension":"3.02.6"}]}}', ''),
	(912, '2020-08-18 17:21:20.046', 'A024', 'e065b603-b8e1-4e6a-a0d4-f129b95ff520', '2020-08-18 17:21:20.045', '2020-08-18 17:21:20.045', '{"Header":{"SessionID":"e065b603-b8e1-4e6a-a0d4-f129b95ff520","FunctionID":"A024","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:21:20 041","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:21:20 041"},"ResponseInfo":{"Result":"OK"}}', ''),
	(913, '2020-08-18 17:21:20.053', 'A019', 'ad01d961-fd7e-40bd-a16b-6b10c447f529', NULL, '2020-08-18 17:21:20.050', '{"Header":{"SessionID":"ad01d961-fd7e-40bd-a16b-6b10c447f529","PCName":"NMRD-I94974-N","FunctionID":"A019","EQCode":"ACOA0022","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:21:20 050"},"RequestInfo":{"ParentEQStateCode":"0","AndonState":"Stop","ChildEQ":[],"Quantity":"0"}}', ''),
	(914, '2020-08-18 17:21:20.057', 'A020', 'ad01d961-fd7e-40bd-a16b-6b10c447f529', '2020-08-18 17:21:20.056', '2020-08-18 17:21:20.056', '{"Header":{"SessionID":"ad01d961-fd7e-40bd-a16b-6b10c447f529","FunctionID":"A020","PCName":"NMRD-I94974-N","EQCode":"ACOA0022","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:21:20 050","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:21:20 052"},"ResponseInfo":{"Result":"OK"}}', ''),
	(915, '2020-08-18 17:21:56.960', 'A001', 'd0a4c25d-ef10-4974-b976-5416eae5fd0f', NULL, '2020-08-18 17:21:56.955', '{"Header":{"SessionID":"d0a4c25d-ef10-4974-b976-5416eae5fd0f","FunctionID":"A001","PCName":"NMRD-I94974-N","EQCode":"ACOA0023","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:21:56 955"},"RequestInfo":{"PCName":"NMRD-I94974-N","EQCode":"ACOA0023"}}', ''),
	(916, '2020-08-18 17:21:56.967', 'A002', 'd0a4c25d-ef10-4974-b976-5416eae5fd0f', '2020-08-18 17:21:56.965', '2020-08-18 17:21:56.965', '{"Header":{"SessionID":"d0a4c25d-ef10-4974-b976-5416eae5fd0f","FunctionID":"A002","PCName":"NMRD-I94974-N","EQCode":"ACOA0023","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:21:56 955","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:21:56 959"},"ResponseInfo":{"Result": "OK"}}', ''),
	(917, '2020-08-18 17:21:57.998', 'A039', '7e7891fb-7efa-43f6-87bf-06e66cd31a7b', NULL, '2020-08-18 17:21:57.984', '{"Header":{"SessionID":"7e7891fb-7efa-43f6-87bf-06e66cd31a7b","FunctionID":"A039","EQCode":"ACOA0023","SoftName":"ATL_MES.exe","RequestTime":"2020-08-18 17:21:57 984","PCName":"NMRD-I94974-N"},"RequestInfo":{"UserID":"superAdmin","UserPassword":"SuperAdmin","TYPE":""}}', ''),
	(918, '2020-08-18 17:21:58.002', 'A040', '7e7891fb-7efa-43f6-87bf-06e66cd31a7b', '2020-08-18 17:21:58.002', '2020-08-18 17:21:58.002', '{"Header":{"SessionID":"7e7891fb-7efa-43f6-87bf-06e66cd31a7b","FunctionID":"A040","PCName":"NMRD-I94974-N","EQCode":"ACOA0023","SoftName":"ServerSoft","RequestTime":"2020-08-18 17:21:57 984","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"","ResponseTime":"2020-08-18 17:21:57 998"},"ResponseInfo":{"UserID":"000777665","UserName":"1234","UserLevel":"Administrator"}}', '');
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
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='离线数据缓存，待上传';

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
	(142, '注液机上位机', 'DeviceControl.WinformMainPage', 'pack://application:,,,/ATL_MES;component/HBinjectPage.xaml', NULL, 3, 140, 2),
	(143, 'CCD', 'DeviceControl.WinformUserControlPage', 'pack://application:,,,/ATL_MES;component/HBvisionPage.xaml', NULL, 4, 140, 2);
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
	('C', 16, 'omron'),
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
	(1, 'PLC1', 'FEF', '192.168.1.40', '102', 'S7Net_1200', NULL, 1);
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
  `remark` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ProtocolName`),
  KEY `ProtocolName` (`ProtocolName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='PLC通信协议';

-- Dumping data for table ptf.plc_protocol: ~19 rows (大约)
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
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8 COMMENT='PLC读写区域设置';

-- Dumping data for table ptf.plc_rw_config: ~17 rows (大约)
/*!40000 ALTER TABLE `plc_rw_config` DISABLE KEYS */;
INSERT INTO `plc_rw_config` (`plc_rw_area_did`, `name`, `plcID`, `areaName`, `startAddress`, `length`, `rw`, `cycle`, `lastTime`, `enabled`) VALUES
	(1, '设备output数据', 1, 'DB', '1000.0', 204, 'R', 2000, '2019-07-18 21:33:00', 1),
	(2, '物料验证动作请求', 1, 'DB', '1000.204', 48, 'R', 1000, '2019-07-18 21:33:00', 1),
	(3, '发送设备状态数据', 1, 'DB', '1000.252', 50, 'R', 1000, '2019-09-26 14:05:00', 1),
	(4, '发送设备报警数据', 1, 'DB', '1000.302', 60, 'R', 1000, '2019-09-19 16:50:00', 1),
	(5, '发送设备关键件寿命数据', 1, 'DB', '1000.362', 192, 'R', 1000, '2019-09-24 09:56:00', 1),
	(6, '发送设备履历数据', 1, 'DB', '1000.554', 264, 'R', 1000, '2019-10-21 09:37:00', 1),
	(7, 'MES主动采集数据', 1, 'DB', '1000.818', 204, 'R', 1000, '2019-10-21 09:40:00', 1),
	(8, '发送测量设备点检数据', 1, 'DB', '1000.1022', 60, 'R', 1000, '2019-10-21 09:41:00', 1),
	(9, '设备(伺服)参数上传', 1, 'DB', '1000.1082', 288, 'R', 1000, '2019-10-21 09:44:00', 1),
	(10, 'input参数及上下限回读', 1, 'DB', '1000.1370', 264, 'R', 1000, '2019-10-21 09:45:00', 1),
	(11, '接收定时信号', 1, 'DB', '2000.0', 52, 'W', 1000, '2019-10-21 09:45:00', 1),
	(12, '接收设备控制指令1', 1, 'DB', '2000.52', 6, 'W', 1000, '2019-10-21 09:46:00', 1),
	(13, '初始化开机指令（input参数）', 1, 'DB', '2000.142', 264, 'R', 1000, '2019-10-21 09:46:00', 1),
	(14, '关键件寿命设定值', 1, 'DB', '2000.406', 264, 'R', 1000, '2019-12-24 08:32:32', 1),
	(15, '特殊设备控制指令(读写一次)', 1, 'DB', '2000.58', 10, 'R', 1000, '2019-10-21 09:46:00', 1),
	(16, '接收设备控制指令2', 1, 'DB', '2000.68', 74, 'W', 1000, '2019-10-21 09:46:00', 1),
	(24, 'test', 1, 'DB', '2000.700', 4, 'W', 1000, '2019-10-21 09:46:00', 1);
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='当前生产概况数据表\r\nisBaking字段表示是否为Baking设备，如果是，值为1，则每个炉子一行数据\r\nInAllCount表示来料总数量，InOKallCount表示来料可拿来生产的总数量(扣减了marking不良等)\r\nOutAllCount表示当前已经下料的总数量，NG1Count表示不良品1数量，NG2Count表示不良品2数量';

-- Dumping data for table ptf.probably: ~0 rows (大约)
/*!40000 ALTER TABLE `probably` DISABLE KEYS */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='每个工艺的设备的电池数据会不一致，需要各个厂家自己设计此表\r\n必须要有EquipmentID、条码字段ProductSN、品质结果字段Model、NgCode、生产时间字段ProductDate、InTime、OutTime、Operator、ResourceShift字段\r\n生产时间字段ProductDate的内容必填\r\n对于有上料扫码的设备，则InTime字段内容必填，表示上料扫码的时间\r\n对于能够知道电池下料时候的时间的设备，则OutTime字段内容必填，表示电池离开时候的时间。\r\nOperator表示当前登录MES账户\r\nResourceShift表示MES下发的当前班次号';

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='每个班次不同NG原因数量统计';

-- Dumping data for table ptf.production_ngcode_statistics: ~0 rows (大约)
/*!40000 ALTER TABLE `production_ngcode_statistics` DISABLE KEYS */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='班次产量统计';

-- Dumping data for table ptf.production_statistics: ~0 rows (大约)
/*!40000 ALTER TABLE `production_statistics` DISABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.roles: ~0 rows (大约)
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` (`roleId`, `roleName`, `MesUserLevel`, `UserLevelPLCValue`, `permissionCodes`, `createTime`, `modifyTime`, `remark`) VALUES
	(0, 'Development Authority', 'Administrator', 408, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:21:52', '超级管理员,禁止删除'),
	(1, 'Maintain Authority', 'Maintainer', 307, 'ALL', '2019-07-02 11:04:39', '2020-08-18 17:21:52', '系统管理员'),
	(2, 'Operator Authority', 'Operator', 206, 'UserManager,UserManager.Personal,UserManager.User', '2019-07-09 09:50:00', '2020-08-18 17:21:52', 'ME人员'),
	(10, 'Guest Authority', 'Guest', 206, 'DeviceOverview,DeviceOverview.ProductOverview,DeviceOverview.Monitor,DeviceOverview.DataCapacityStatistics,DeviceOverview.NGStatistics,DeviceOverview.AlarmStatistics,DeviceOverview.PC-PLCrealtimeData,DeviceOverview.Version,DeviceOverview.Start,MES,MES.MESweb,Maintain,DataWareHouse,SystemSetting,LogQuery,UserManager,UserManager.Login,DeviceControl', '2019-07-02 11:04:39', '2020-08-18 17:21:52', '尚未登陆');
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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Dumping data for table ptf.users: ~0 rows (大约)
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`userId`, `userName`, `password`, `name`, `roleId`, `createTime`, `gender`, `LastLoginTime`, `remark`) VALUES
	(1, 'admin', 'admin', 'admin', 1, '2017-03-23 21:05:21', 1, '2019-07-09 08:20:38', NULL),
	(2, 'Guest', 'Guest', 'Guest', 10, '2017-03-23 21:05:21', 1, '2020-08-18 17:21:51', NULL),
	(4, 'SuperAdmin', 'SuperAdmin', 'SuperAdmin', 0, '2019-07-08 13:22:06', 0, '2019-12-27 10:30:29', NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=171 DEFAULT CHARSET=utf8 COMMENT='用户定义变量表。variableTypeID等于1表示为系统变量，系统变量与PLC变量，不是在同一个UI界面上显示的。\r\n系统变量不会与PLC地址绑定。非系统变量可能会与PLC地址绑定关联，用于接口数据上传。\r\n每个MES ID的变量关联的plc_rw_area_did都必须是Ｒ读类型的．MES ID的变量属于OUTPUT参数，上传给mes的．needMonitorLog都必须为１\r\nmes input参数变量也全部needMonitorLog必须是1';

-- Dumping data for table ptf.user_define_variable: ~0 rows (大约)
/*!40000 ALTER TABLE `user_define_variable` DISABLE KEYS */;
INSERT INTO `user_define_variable` (`variableID`, `variableName`, `variableTypeID`, `value`, `description`, `valueTypes`, `variableLength`, `plc_rw_area_did`, `plc_address`, `needMonitorLog`, `remark`, `datatime`) VALUES
	(3, 'ClearProductionData', 1, '30', '生产记录数据保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:21:52'),
	(4, 'ClearOperationLog', 1, '30', '操作日志保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:21:52'),
	(5, 'ClearInputUploadHistory', 1, '60', 'Output上载历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-23 20:56:09'),
	(6, 'ClearInputDownloadHistory', 1, '180', 'Input下发历史记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-23 20:56:06'),
	(7, 'ClearLog4net', 1, '30', '软件运行日志保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-12-23 20:56:01'),
	(8, 'ClearHmiSettingMonitorLog', 1, '30', '重点监控参数保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:56:24'),
	(9, 'ClearMESinterfaceLog', 1, '3', 'MES接口调用记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-03-27 11:08:16'),
	(10, 'ClearAlarmRecord', 1, '31', '历史报警记录保存时间', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:21:52'),
	(11, 'ClearPLCinteractiveLog', 1, '31', 'PC/PLC交互记录保存时间(天)', 'Int16', 1, NULL, NULL, b'0', NULL, '2019-11-21 18:51:01'),
	(12, 'dayShiftStartTime', 1, '7:30', '白班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(13, 'nightShiftStartTime', 1, '19:31', '夜班上班时间', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:45'),
	(14, 'AssetsNO', 1, 'ACOA0023', '设备编号', 'String', 1, NULL, NULL, b'0', '不可删除此', '2019-12-27 10:10:23'),
	(27, 'Model', 1, 'M20', '生产型号', 'String', 1, NULL, NULL, b'0', NULL, '2019-08-26 15:02:50'),
	(29, 'PLCversion', 2, '3.87.895', 'PLC程序版本号', 'String', 22, 6, 'DB1000.554', b'1', NULL, '2020-03-27 11:08:16'),
	(30, 'HMIversion', 2, '1.25.3544', 'HMI版本', 'String', 22, 6, 'DB1000.576', b'1', NULL, '2020-08-18 17:21:52'),
	(31, 'Lengthversion', 2, '1.25.3544', '测长程序版本号', 'String', 22, 6, 'DB1000.598', b'1', NULL, '2020-03-27 11:08:16'),
	(32, 'CCDVersion', 1, '3.02.6', 'CCD软件版本号', 'String', 1, NULL, NULL, b'0', NULL, '2019-09-24 19:14:34'),
	(33, 'ModelVersion', 2, '3.02.6', 'Model号', 'String', 22, 6, 'DB1000.620', b'0', NULL, '2020-03-27 11:08:16'),
	(34, 'MESSoftware', 2, '3.02.6', '上位机程序版本号', 'String', 22, 6, 'DB1000.642', b'0', NULL, '2020-03-27 11:08:16'),
	(45, 'DefaultMesUrl', 1, 'https://www.baidu.com/', '默认MES网址', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-25 18:47:15'),
	(46, 'ControlCode', 2, '0', 'mes控机操作的PLC地址', 'Int16', 1, 12, 'DB2000.52', b'0', 'FK', '2020-08-18 17:21:52'),
	(47, 'MESstatusToPLC', 2, '10', 'MES连接状态toPLC', 'Int16', 1, 11, 'DB2000.2', b'0', 'FK', '2020-03-27 11:08:16'),
	(48, 'HeatBeat', 2, '10', '心跳', 'Float', 1, 16, 'DB2000.80', b'0', 'FK', '2020-08-18 17:21:52'),
	(49, 'ParentEQState', 2, '0', '设备状态', 'Int16', 1, 3, 'DB1000.252', b'0', 'FK', '2020-08-18 17:21:52'),
	(50, 'AndonState', 2, '0', 'Andon状态', 'Int16', 1, 3, 'DB1000.254', b'0', 'FK', '2020-08-18 17:21:52'),
	(51, 'Quantity', 2, '0', '当班产量', 'Int16', 1, 3, 'DB1000.256', b'0', 'FK', '2020-08-18 17:21:52'),
	(52, 'HmiPermissionRequest', 2, '0', 'PLC获取HMI权限触发', 'Int16', 1, 3, 'DB1000.262', b'0', 'FK', '2020-08-18 17:21:52'),
	(53, 'Account', 2, '0', 'PLC账户', 'String', 6, 3, 'DB1000.264', b'0', 'FK', '2020-08-18 17:21:52'),
	(54, 'Code', 2, '0', 'PLC账户密码', 'String', 6, 3, 'DB1000.268', b'0', 'FK', '2020-08-18 17:21:52'),
	(55, 'UserLevel', 2, '0', 'PLC账户等级', 'Int16', 1, 16, 'DB2000.68', b'0', 'FK', '2020-08-18 17:21:52'),
	(56, 'MesReply', 2, NULL, 'MES响应状态', 'Int16', 1, 11, 'DB2000.4', b'0', 'FK', '2020-08-18 17:21:52'),
	(57, 'A007Count', 2, '0', 'A007首件数量', 'Int16', 1, 12, 'DB2000.56', b'0', 'FK', '2020-03-27 11:08:16'),
	(58, 'StateCode', 2, '0', 'A007首件信号', 'Int16', 1, 12, 'DB2000.54', b'0', 'FK', '2020-03-27 11:08:16'),
	(60, 'test', 2, NULL, 'test', 'Bit', 1, 24, 'DB2000.700.0', b'0', NULL, '2020-03-27 11:08:16'),
	(65, 'localIpAdr', 1, '127.0.0.1', '本机 IP地址', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-24 08:52:22'),
	(67, 'localUdpSendPortNo', 1, '61000', '本机udp发送端口', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-26 09:44:06'),
	(68, 'localUdpRecvPortNo', 1, '50002', '本机udp接收端口号', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-26 09:44:04'),
	(69, 'localTcpPortNo', 1, '60001', '本机tcp接收端口号', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-24 08:52:25'),
	(70, 'serverIpAdr', 1, '127.0.0.1', 'MES服务端IP地址', 'String', 1, NULL, NULL, b'0', NULL, '2019-12-24 08:52:26'),
	(71, 'serverUdpPortNo', 1, '50000', 'MES服务端udp接收端口号', 'Int32', 1, NULL, NULL, b'0', NULL, '2019-12-24 08:52:27'),
	(161, 'AutoPopMonitorPage', 1, '1', '是否启用电脑无操作几分钟后自动弹出监控画面', 'Int16', 1, NULL, NULL, b'0', NULL, '2020-03-27 11:08:16'),
	(167, 'LabChinese', 1, 'FEF', '工序名称(中文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-03-27 11:09:42'),
	(168, 'LabEnglish', 1, 'ALI-FEF', '工序名称(英文)', 'String', 1, NULL, NULL, b'0', NULL, '2020-03-27 11:09:42'),
	(169, 'LabVersion', 1, 'V1.1.0001', '版本号', 'String', 1, NULL, NULL, b'0', NULL, '2020-03-27 11:09:42'),
	(170, 'DefaultAcount', 1, 'superAdmin', 'Default Acount', 'String', 1, NULL, NULL, b'0', NULL, '2020-08-18 17:21:52');
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
