import React from 'react'
import PropTypes from 'prop-types'
import { Button } from '../../button'

export const Submit = ({ submit, value, className }) =>
	<Button
		variant="cta"
		onClick={() => submit()}
		className={className}
	>
		<h1>{value}</h1>
	</Button>

Submit.propTypes = {
	value: PropTypes.string,
	onSubmit: PropTypes.func,
}