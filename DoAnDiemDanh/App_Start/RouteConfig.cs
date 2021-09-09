using DoAnDiemDanh.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAnDiemDanh
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			//routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//routes.MapRoute(
			//    name: "Default",
			//    url: "{controller}/{action}/{id}",
			//    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			//);

			routes.MapRoute(
				name: "Default", url: "{controller}/{action}/{id}",
				defaults: new { controller = "DiemDanh", action = "Index", id = UrlParameter.Optional },
				constraints: new { TenantRouting = new RoutingConstraint() }
					 );
		}
	}
	public class RoutingConstraint : IRouteConstraint // It is main Class for Multi teanant
	{

		public bool Match(HttpContextBase httpContext, Route route, string getParameter, RouteValueDictionary values, RouteDirection routeDirection)
		{
		
			var GetAddress = httpContext.Request.Headers["Host"].Split('.');
			var tenant = GetAddress[0];
			
			if (GetAddress.Length < 2) 
			{
				tenant = "This is the main domain";

				Constant.DatabaseName = "FACE_RECOGNITION_V2";
				if (!values.ContainsKey("tenant"))
					values.Add("tenant", tenant);
				//return false;
				// return true;
			}
			else if (GetAddress.Length == 2) 
			{
				tenant = "This is the main domain when run iis";

				Constant.DatabaseName = GetAddress[0];
				
				if (!values.ContainsKey("tenant"))
					values.Add("tenant", tenant);
				//return false;
				// return true;
			}
			else if (!values.ContainsKey("tenant")) 
			{
				values.Add("tenant", tenant);
				Constant.DatabaseName = GetAddress[1] + "_" + tenant;
			}

			return true;
		}
	}
}
