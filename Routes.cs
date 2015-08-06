using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace MainBit.Users
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                            // higher priority than Profile/{username} as Edit could be interpreted as a username
                             new RouteDescriptor {   Priority = 6,
                                                     Route = new Route(
                                                         "private/profile",
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Users"},
                                                                                      {"controller", "Private"},
                                                                                      {"action", "Index"},

                                                         },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Users"}
                                                                                  },
                                                         new MvcRouteHandler())

                             },
                             new RouteDescriptor {   Priority = 5,
                                                     Route = new Route(
                                                         "profiles/{id}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Users"},
                                                                                      {"controller", "Profile"},
                                                                                      {"action", "Index"},

                                                         },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Users"}
                                                                                  },
                                                         new MvcRouteHandler())

                             },

                             new RouteDescriptor {   Priority = 6,
                                                     Route = new Route(
                                                         "private/change-password",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Users"},
                                                                                      {"controller", "Account"},
                                                                                      {"action", "ChangePassword"},

                                                         },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Users"}
                                                                                  },
                                                         new MvcRouteHandler())

                             },

                             new RouteDescriptor {   Priority = 6,
                                                     Route = new Route(
                                                         "private/change-password-success",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Users"},
                                                                                      {"controller", "Account"},
                                                                                      {"action", "ChangePasswordSuccess"},

                                                         },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Users"}
                                                                                  },
                                                         new MvcRouteHandler())

                             },
                         };
        }
    }
}