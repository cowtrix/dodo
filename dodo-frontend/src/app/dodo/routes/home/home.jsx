import React from 'react'
import PropTypes from 'prop-types'
import { Header } from './header'
import { Rebellions } from './rebellions'


export const Home = ({ getRebellions }) => {
	getRebellions()
	return (
		<div>
			<Header/>
			<Rebellions/>
		</div>
	)
}


Home.propTypes = {
	getRebellions: PropTypes.func,
}
