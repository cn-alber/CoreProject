// We only need to import the modules necessary for initial render
import WheatLayout from 'layouts/WheatLayout'
import PageLayout from 'layouts/PageLayout'
import DashBordRoute from './DashBord'
import ShopRoute from './Shop'
import TestRoute from './Test'
import LoginRoute from './Login'
import ApplyRoute from './Apply'
//import LockRoute from './Lock'
import NotFoundRoute from './NotFound'

export const createRoutes = (store) => ([
  {
    path: '/go',
    component: PageLayout,
    childRoutes: [
      LoginRoute(store),
      ApplyRoute(store),
      NotFoundRoute
    ],
    ignoreScrollBehavior: true
    //indexRoute: LoginRoute(store)
  },
  {
    path: '/',
    component: WheatLayout,
    indexRoute: DashBordRoute(store),
    childRoutes: [
      ShopRoute(store),
      TestRoute(store),
      NotFoundRoute
    ],
    ignoreScrollBehavior: true
  }
  // {
  //   path: '*',
  //   component: PageLayout,
  //   indexRoute: NotFoundRoute,
  //   childRoutes: [
  //     //LockRoute(store)
  //   ]
  // }
])

/*  Note: childRoutes can be chunked or otherwise loaded programmatically
    using getChildRoutes with the following signature:

    getChildRoutes (location, cb) {
      require.ensure([], (require) => {
        cb(null, [
          // Remove imports!
          require('./Counter').default(store)
        ])
      })
    }

    However, this is not necessary for code-splitting! It simply provides
    an API for async route definitions. Your code splitting should occur
    inside the route `getComponent` function, since it is only invoked
    when the route exists and matches.
*/

export default createRoutes
