import React, {Fragment} from 'react'
import { Route } from 'react-router-dom'
import { Home, route as home } from './home'

export const Routes = () =>
	<Fragment>
		<Route path={home} component={Home} />
	</Fragment>
