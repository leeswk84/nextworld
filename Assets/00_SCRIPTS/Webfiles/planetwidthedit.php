<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{   ### 행성 넓이 변환
	if($w_count == null)
    {
        $str = "{\"result\":false,\"msg\":\"";
		$str .= "w_count null";	
		$str .= "\"}";
		echo $str;
    }
    else if($planet == null)
    {
        $str = "{\"result\":false,\"msg\":\"";
		$str .= "planet null";	
		$str .= "\"}";
		echo $str;
    }
    else
	{	
        $sql = "SELECT * FROM NextWorld_planet WHERE planet = '$planet'";
		$result = mysql_query($sql,$dbCon);

        if($result == false)
        {
            echo "{\"result\":false,\"msg\":\"planet spl error\"}";
        }
        else if(mysql_num_rows($result) == 0)
		{   ### 없으면 에러..
            echo "{\"result\":false,\"msg\":\"planet not\"}";
		}
        else
        {   ### 있으면 수정
            $sql = "UPDATE NextWorld_planet SET w_count = '$w_count' WHERE planet = '$planet'";
            mysql_query($sql,$dbCon);

            echo "{\"result\":true}";
        }
	}
}

mysql_close($dbCon);
?>