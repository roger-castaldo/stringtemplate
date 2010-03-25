<table>
	<tr>
		<td colspan="3">
			<img src="/resources/icons/phone_add.png" style="cursor:pointer;display:inline;" onclick="ShowAddExtension(this);"/>Add Extension
			<select style="display:none;" name="extensions">
				<option value=""></option>
				$extensions:{ext |
					<option value="$ext.Number$">
						$if(ext.InternalCallerIDName)$
							$ext.InternalCallerIDName$ ($ext.Number$)
						$else$
							$ext.Number$
						$endif$
					
					</option>
				}$
			</select>
		</td>
	</tr>
	<tr style="display:none;">
		<td></td>
		<td colspan="2">
			Extension: <input type="text" name="Extension"/><br/>
			Password: <input type="text" name="password"/><br/>
			Default Context: <select name="context">
				$contexts:{con |
					<option value="$con.Name$">$con.Name$</option>
				}$
			</select><br/>
			Internal Caller ID Name: <input name="InternalCallerID" type="text"/><br/>
			Internal Caller ID Number: <input name="InternalCallerNumber" type="text"/><br/>
			External Caller ID Name: <input name="ExternalCallerID" type="text"/><br/>
			External Caller ID Number: <input name="ExternalCallerNumber" type="text"/><br/>
			Direct Number: <input type="text" name="DirectNumber"/><br/>
			<span style="cursor:pointer;" onclick="CreateExtension(this);"><img src="/resources/icons/accept.png" style="display:inline;margin-right:5px;">Add Extension</span>
			<span style="cursor:pointer;margin-left:5px;" onclick="HideAddExtension(this);"><img src="/resources/icons/cancel.png" style="display:inline;margin-right:5px;">Cancel</span>
		</td>
	</tr>
	<tr>
		<th style="width:100px;">Extension #</th>
		<th>Caller ID Name</th>
		<th>Actions</th>
	</tr>
	$extensions:{ext |
		<tr>
			<td>$ext.Number$</td>
			<td>$ext.InternalCallerIDName$</td>
			<td>
				<img src="/resources/icons/phone_edit.png" style="cursor:pointer;padding-left:10px;" title="Edit Extension Settings" onClick="EditExtension(this, '$ext.Number', '$replace($ext.Password$,',\')$', '$ext.UserContext.Name$','$replace($ext.InternalCallerIDName$,',\')$','$ext.InternalCallerID$','$replace($ext.ExternalCallerIDName$,',\')$','$ext.ExternalCallerID$','$ext.DirectLine$');"/>
				$if(ext.Mail)$
					<img src="/resources/icons/email_edit.png" style="cursor:pointer;padding-left:10px;" title="Edit Voicemail settings" onClick="UpdateVoicemail(this,'$ext.Number$','$replace($ext.Mail.Password$,',\')$','$ext.Mail.Email$',$if(ext.Mail.MaxMessage)$$ext.Mail.MaxMessage$$else$null$endif$,$if(ext.Mail.CopyTo)$'$ext.Mail.CopyTo.Number$'$else$null$endif$,$if(ext.Mail.AttachToEmail)$true$else$false$endif$);"/>
					<img src="/resources/icons/email_delete.png" style="cursor:pointer;padding-left:10px;" title="Delete Voicemail" onClick="DeleteVoicemail(this,'$ext.Number$');/>
				$else$
					<img src="/resources/icons/email_add.png" style="cursor:pointer;padding-left:10px;" title="Add Voicemail to Extension" onClick="CreateVoicemail(this,'$ext.Number$');"/>
				$endif$					
			</td>
		</tr>
	}$
</table>