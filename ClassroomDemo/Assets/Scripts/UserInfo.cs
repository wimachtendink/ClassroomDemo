using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
	public enum UserRole
	{
		none				= 0,
		audit				= 1,
		student				= 2,
		teachingAssistant	= 3,
		professor			= 4,
	}

	public string name;
	public UserRole userRole;


	public void SetUserRoleStudent()
	{
		userRole = UserRole.student;
	}

	public void SetUserRoleProfessor()
	{
		userRole = UserRole.professor;
	}


	//for ease of UnityEvents without having to make a UnityEvent<UserRole> then wrap it in another unity event for use with buttons, lol
	public void SetUserRole(int userRole)
	{
		SetUserRole((UserRole)userRole);
	}

	public void SetUserRole(UserRole userRole)
	{
		this.userRole = userRole;
	}

}
