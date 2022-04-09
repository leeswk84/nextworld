<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 맵데이터 목록
	if($planet != null && $tiles != null)
	{	 
		//if($current == null) $current = 0;
		//if($page == null) $page = 1;
		//$sql = "SELECT * FROM NextWorld_map WHERE planet = '$planet' ORDER BY tile LIMIT $current, $page";
		
        $sql = "SELECT * FROM NextWorld_planet WHERE planet = '$planet'";
        $result = mysql_query($sql,$dbCon);
        
        if($result == false || mysql_num_rows($result) == 0)
		{
			echo "{\"result\":false,\"msg\":\"none list count\"}";
		}
        else
        {
            $row = mysql_fetch_array($result);
            $count = $row[count];
            $w_count = $row[w_count];
            $h_count = $row[h_count];

            $sql = "SELECT * FROM NextWorld_map WHERE planet = '$planet' AND tile IN ($tiles)";
            $result = mysql_query($sql,$dbCon);  
		
		    if($result == false)
            {
                echo "{\"result\":false,\"msg\":\"sql error\"}";
            }
            else if(mysql_num_rows($result) == 0)
		    {
			    echo "{\"result\":false,\"msg\":\"none list\"}";
		    }
		    else
		    {	# 데이터 있는 경우
			    $i=0;
			    $str = "{\"result\":true,\"planet\":".$planet.",\"count\":".$count.",\"width\":".$w_count.",\"height\":".$h_count.",\"tiles\":{";
			    while($row = mysql_fetch_array($result))
			    {
				    if($i != 0) $str .= ",";
				    $i++;
				    $str .= "\"".$row[tile]."\":{";
    //				$str .= "\"idx\":".$row[idx];
				    $str .= "\"dt\":".$row[maptype];
				    $str .= ",\"dg\":\"".$row[dataground]."\"";
				    $str .= ",\"dr\":\"".$row[dataroad]."\"";
				    $str .= ",\"db\":\"".$row[databuild]."\"";
				    $str .= "}";
			    }
			    $str .= "}}"; 
			    echo $str;
		    }	
        }
	}	
	else
	{
		echo "{\"result\":false,\"msg\":\"not planet\"}";	
	}
}

mysql_close($dbCon);
?>