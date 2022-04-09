<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 맵데이터 수정
	if($tiles != null && $planet != null && $maptype)
	{	
		$sql = "UPDATE NextWorld_map SET maptype = '$maptype' WHERE planet = '$planet' AND tile IN ($tiles)";
		$result = mysql_query($sql,$dbCon);
		
		if($result == false)
		{
			echo "{\"result\":false,\"msg\":\"spl error\"}";
		}
		else
		{
			echo "{\"result\":true".
					",\"planet\":".$planet.
					",\"maptype\":".$maptype.
                    "}";
		}
	}	
	else
	{
		$str = "{\"result\":false,\"msg\":\"";
		if($planet == null) $str .= "planet null ";	
		if($tiles == null) $str .= "tiles null ";	
		if($maptype == null) $str .= "maptype null ";	
        $str .= "\"}";
		
		echo $str;
	}
}

//http://www.leeswk.com/wk_admin/php/nextworld/mapedittype.php?tiles=0,1&planet=1&maptype=1

mysql_close($dbCon);
?>