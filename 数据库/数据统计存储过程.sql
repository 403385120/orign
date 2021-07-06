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

-- Dumping structure for event ptf.daily_work
DELIMITER //
CREATE DEFINER=`root`@`localhost` EVENT `daily_work` ON SCHEDULE EVERY 12 HOUR STARTS '2019-11-29 00:00:00' ON COMPLETION PRESERVE ENABLE COMMENT '每日0点执行事件' DO begin
CALL `production_analysis_daily`();
end//
DELIMITER ;

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

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
