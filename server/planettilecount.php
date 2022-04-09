<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 행성 구간 갯수, 넓이, 높이 반환
	if($planet != null )
	{	
		$sql = "SELECT * FROM NextWorld_planet WHERE planet = '$planet'";
		$result = mysql_query($sql,$dbCon);
		
		if($result == false)
		{
			echo "{\"result\":false,\"msg\":\"spl error\"}";
		}
        else if(mysql_num_rows($result) == 0)
        {
            echo "{\"result\":false,\"msg\":\"list not\"}";
        }
		else
		{
            $row = mysql_fetch_array($result);
			
			echo "{\"result\":true".
					",\"planet\":".$planet.
					",\"count\":".$row[count].
                    ",\"width\":".$row[w_count].
					",\"height\":".$row[h_count].
					"}";
		}
	}	
	else
	{
		$str = "{\"result\":false,\"msg\":\"";
		$str .= "planet null";	
		$str .= "\"}";
		echo $str;
	}
}

//http://www.leeswk.com/wk_admin/php/nextworld/planettilecount.php?planet=1

mysql_close($dbCon);
?>