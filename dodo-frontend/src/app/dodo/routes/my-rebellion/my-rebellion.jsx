import React from 'react'
import PropTypes from 'prop-types'
import { Container } from 'app/components/forms'
import { Resource } from './resource'


const MY_REBELLION = "My rebellion"

export const MyRebellion = ({ memberOf = [], resourceTypes }) =>
	<Container
		title={MY_REBELLION}
		content={
			memberOf.map(resource =>
				<Resource {...resource} resourceTypes={resourceTypes} />
			)
		}
	/>


MyRebellion.propTypes = {
	user: PropTypes.object
}