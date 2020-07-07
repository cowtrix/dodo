import React, { Fragment } from "react"
import { Route } from "react-router-dom"
import { Switch } from "react-router"

import { Search, SEARCH_ROUTE as search } from "./search"
import { Resource } from "./resource"
import { Login } from "./login"
import { NotFound } from "./error"
import { ResetPassword } from './reset-password'
import { Register } from './register'

export const Routes = () => (
	<Fragment>
		<Switch>
			<Route path={"/:eventType/:eventId"} component={Resource} exact />
			<Route path={search} component={Search} exact />
			<Route path={"/login"} component={Login} exact />
			<Route path={"/reset-password"} component={ResetPassword} exact />
			<Route path={"/register"} component={Register} exact />
			<Route path="*" component={NotFound} />
		</Switch>
	</Fragment>
)
