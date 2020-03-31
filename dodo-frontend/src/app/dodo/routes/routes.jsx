import React, {Fragment} from 'react'
import { Route } from 'react-router-dom'
import { Home, route as home } from './home'
import { Switch } from "react-router"
import { Rebellion } from "app/dodo/routes/rebellion"


export const Routes = () =>
	<Fragment>
		<Switch>
			<Route
				path={"/rebellion/:rebellionId"}
				component={Rebellion}
				exact
			/>
			<Route path={home} component={Home} />
		</Switch>
	</Fragment>

