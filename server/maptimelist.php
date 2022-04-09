<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
    ### 맵데이터 목록
    if($planet != null && $tiles != null)
    {	 
        $sql = "SELECT * FROM NextWorld_map WHERE planet = '$planet' AND tile IN ($tiles)";
        $result = mysql_query($sql,$dbCon);  
		
        if($result == false || mysql_num_rows($result) == 0)
        {
	        echo "{\"result\":false,\"msg\":\"none list\"}";
        }
        else
        {	# 데이터 있는 경우
	        $i=0;
	        $str = "{\"result\":true,\"planet\":".$planet.",\"tiles\":{";
	        while($row = mysql_fetch_array($result))
	        {
		        if($i != 0) $str .= ",";
		        $i++;
		        $str .= "".$row[tile].":{";
		        $str .= "\"time\":".$row[updatedt];
		        $str .= "}";
	        }
	        $str .= "}}"; 
	        echo $str;
        }	
	}	
	else
	{
		echo "{\"result\":false,\"msg\":\"not planet\"}";	
	}
}

mysql_close($dbCon);
?>