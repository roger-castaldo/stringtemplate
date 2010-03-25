<table id="\$id\$_holdMusicList" cellspacing="0" cellpadding="3">
	<tr>
		<th>Name</th>
	</tr>
	$files:{file | 
		<tr id="\$id\$_$i$">
			<td>$file.Name$</td>
			<td>
				<span style="cursor:pointer;padding-right:5px;" onClick="RegisterFileUploadButton(this,'/FileUpload.ashx','$file.JScriptName$',null,null,function(file,result){UpdateMusicOnHoldFile('\$id\$',result,'$file.JScriptName$');});\$(this).css('cursor','pointer');">
					<img src="/resources/icons/arrow_up.png" style="padding-right:5px;"> Update
				</span>
				<span style="cursor:pointer;padding-right:5px;" onClick="ca_homehosting_FreeSwitchConfig_Site_Core_Services_FileAccessService.DeleteFile('$file.JScriptName$',function(msg){if (msg){alert('Successfully deleted file $file.JScriptName$');\$('#\$id\$_$i$').remove();}else{alert('An error occured in attempting to delete the file $file.JScriptName$');}},function(msg){alert('An error occured in contacting the File Access Service.');});">
					<img src="/resources/icons/cancel.png" style="padding-right:5px;"> Delete
				</span>
			</td>
		</tr>
	}$
</table>
<span id="\$id\$_NewMusic" style="cursor:pointer;padding-right:5px;" onMouseOver="RegisterFileUploadButton(this,'/FileUpload.ashx','NewFile',null,null,function(file,result){AddMusicOnHold('\$id\$',file,result);});\$(this).css('cursor','pointer');">
  <img src="/resources/icons/add.png" title="Add New" style="padding-right:5px;"> Add New Music
</span>