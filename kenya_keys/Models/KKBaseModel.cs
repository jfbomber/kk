using System;

public class KKBaseModel
{
	public KKBaseModel ()
	{

	}
	public static string ConnString { get { return System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ConnectionString; } }

	public string ConnectionString { get { return ConnString; } } 
	public string ErrorMessage { get; set; } 
	public bool HasError {  get { return this.ErrorMessage != String.Empty ? true : false; } }
}

public class KKError { 
	public string _ErrorMessage;


}