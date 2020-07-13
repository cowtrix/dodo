import React, { Fragment } from "react"
import { Route } from "react-router-dom"
import { Switch } from "react-router"

import { Search, SEARCH_ROUTE } from "./search"
import { RESOURCE_ROUTE, Resource } from "./resource"
import { Login, LOGIN_ROUTE } from "./login"
import { NotFound } from "./error"
import { ResetPassword, RESET_PASSWORD_ROUTE } from './reset-password'
import { Register, REGISTER_ROUTE } from './register'
import { MyRebellion, MY_REBELLION_ROUTE } from './my-rebellion'

export const Routes = () => (
	<Fragment>
		<Switch>
			<Route path={RESOURCE_ROUTE} component={Resource} exact />
			<Route path={SEARCH_ROUTE} component={Search} exact />
			<Route path={LOGIN_ROUTE} component={Login} exact />
			<Route path={RESET_PASSWORD_ROUTE} component={ResetPassword} exact />
			<Route path={REGISTER_ROUTE} component={Register} exact />
			<Route path={MY_REBELLION_ROUTE} component={MyRebellion} />
			<Route path="*" component={NotFound} />
		</Switch>
	</Fragment>
)
