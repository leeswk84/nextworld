<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 행성 구간 갯수 변환, 반환
	if($planet == null)
    {
        $str = "{\"result\":false,\"msg\":\"";
		$str .= "planet null";	
		$str .= "\"}";
		echo $str;
    }
	else if($w_count == null)
    {
        $str = "{\"result\":false,\"msg\":\"";
		$str .= "w_count null";	
		$str .= "\"}";
		echo $str;
    }
	else if($h_count == null)
    {
        $str = "{\"result\":false,\"msg\":\"";
		$str .= "h_count null";	
		$str .= "\"}";
		echo $str;
    }
    else
	{	
		$dircnt = $h_count * $w_count;

        $sql = "SELECT * FROM NextWorld_planet WHERE planet = '$planet'";
		$result = mysql_query($sql,$dbCon);

        if($result == false)
        {
            echo "{\"result\":false,\"msg\":\"planet spl error\"}";
        }
        else if(mysql_num_rows($result) == 0)
		{   ### 없으면 새로 생성
            $sql = "INSERT INTO NextWorld_planet VALUES('','$planet','$dircnt','$w_count', '$h_count')";
            mysql_query($sql,$dbCon);
		}
        else
        {   ### 있으면 수정
            $sql = "UPDATE NextWorld_planet SET count = '$dircnt', w_count = '$w_count', h_count = '$h_count' WHERE planet = '$planet'";
            mysql_query($sql,$dbCon);
        }

		### 총갯수 보다 내용이 많으면 삭제
		$sql = "DELETE FROM NextWorld_map WHERE planet = '$planet' AND tile >= '$dircnt'";
        mysql_query($sql,$dbCon);

		$sql = "SELECT * FROM NextWorld_map WHERE planet = '$planet'";
		$result = mysql_query($sql,$dbCon);
		
		if($result == false)
		{
			echo "{\"result\":false,\"msg\":\"spl error\"}";
		}
		else
		{
			$cnt = mysql_num_rows($result);
			
			$date = date("y.m.d H:i:s");

			for($i = $cnt; $i < $dircnt; $i++)
			{
				$sql2 = "INSERT INTO NextWorld_map VALUES('','$planet','$i',0,'','','','$date')";
				$result2 = mysql_query($sql2,$dbCon);
			}

			echo "{\"result\":true".
					",\"planet\":".$planet.
					"}";
		}
	}
}

### http://www.leeswk.com/wk_admin/php/nextworld/planettilecount.php?planet=1

mysql_close($dbCon);
?>