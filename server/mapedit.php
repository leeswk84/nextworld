<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 맵데이터 수정
	if($tile != null && $planet != null )
	{	
		//if($maptype == null) $maptype = 0;
		//if($dataground == null) $dataground = "{}";
		//if($dataroad == null) $dataroad = "{}";
		//if($databuild == null) $databuild = "{}";

		$sql = "SELECT * FROM NextWorld_map WHERE planet = '$planet' AND tile = '$tile'";
		$result = mysql_query($sql,$dbCon);
		
		if($result == false)
		{
			echo "{\"result\":false,\"msg\":\"spl error\"}";
		}
		else
		{
			$date = date("y.m.d H:i:s");

			if(mysql_num_rows($result) == 0)
			{	# 디비에 해당 아이디 데이터 없는 경우
				$sql2 = "INSERT INTO NextWorld_map VALUES('','$planet','$tile',0,'$dataground','$dataroad','$databuild','$date')";
			}
			else
			{	# 데이터 있는 경우
				if($maptype != null)
				{
					$sql2 = "UPDATE NextWorld_map SET maptype = '$maptype', updatedt = '$date' WHERE planet = '$planet' and tile = '$tile'";
                    $result2 = mysql_query($sql2,$dbCon);   
                }
                if($dataground != null)
				{
                    $sql2 = "UPDATE NextWorld_map SET dataground = '$dataground', updatedt = '$date' WHERE planet = '$planet' and tile = '$tile'";
                    $result2 = mysql_query($sql2,$dbCon);   
                }
                if($dataroad != null)
				{
                    $sql2 = "UPDATE NextWorld_map SET dataroad = '$dataroad', updatedt = '$date' WHERE planet = '$planet' and tile = '$tile'";
                    $result2 = mysql_query($sql2,$dbCon);   
                }
                if($databuild != null)
				{
                    $sql2 = "UPDATE NextWorld_map SET databuild = '$databuild', updatedt = '$date' WHERE planet = '$planet' and tile = '$tile'";
                    $result2 = mysql_query($sql2,$dbCon);   
                }

                /*
				else
				{
					$sql2 = "UPDATE NextWorld_map SET dataground = '$dataground', dataroad = '$dataroad', databuild = '$databuild', updatedt = '$date' WHERE planet = '$planet' and tile = '$tile'";
				}*/
			}


			$result = mysql_query($sql,$dbCon);
			$row = mysql_fetch_array($result);
			
			echo "{\"result\":true".
					",\"idx\":".$row[idx].
					",\"planet\":".$planet.
					",\"tile\":".$tile.
					"}";
		}
	}	
	else
	{
		$str = "{\"result\":false,\"msg\":\"";
		if($planet == null) $str .= "planet null ";	
		if($tile == null) $str .= "tile null ";	
		$str .= "\"}";
		
		echo $str;
	}
}

//http://www.leeswk.com/wk_admin/php/nextworld/editmap.php?tile=1&planet=1&dataground={aa:1}&dataroad={v:1}&databuild={b:1}

mysql_close($dbCon);
?>