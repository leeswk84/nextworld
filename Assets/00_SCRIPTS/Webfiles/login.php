<? 
# db접속 및 테이블 common 에서 가져오기

require "../common.php";
dbCon();

if($dbCon)
{	
	### 로그인 하기
	$date = date("y.m.d H:i:s");
	$userID = RTRIM($userID);
	
	if($userID != "")
	{	
		$sql = "SELECT * FROM NextWorld_member WHERE id = '$userID'";
		$result = mysql_query($sql,$dbCon);
		
		if($result == false)
		{
			echo "{\"result\":false}";
		}
		else if(mysql_num_rows($result) == 0)
		{	# 디비에 해당 아이디 데이터 없는 경우
			$sql2 = "INSERT INTO NextWorld_member VALUES('','$userID','','$date','$date','$date','$data')";
			$result2 = mysql_query($sql2,$dbCon);
			echo "{\"result\":true".
					",\"cmd\":\"logincreate\"".
					"}";
		}
		else
		{	# 데이터 있는 경우
			# 시간 업데이트
			$sql2 = "UPDATE NextWorld_member SET logindate = '$date' WHERE id = '$userID'";
			$result2 = mysql_query($sql2,$dbCon);

			$row = mysql_fetch_array($result);
			
			echo "{\"result\":true".
				",\"cmd\":\"loginload\"".
				",\"nick\":\"".$row[nick]."\"".
				",\"data\":".$row[data].
				 "}";
		}
	}	
	else
	{
		echo "{\"result\":false}";	
	}
}

mysql_close($dbCon);
?>