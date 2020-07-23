import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import styles from '../input/input.module.scss'

export const TextArea = ({ value, setValue, id, name, className }) =>
	<div className={styles.textArea}>
		<textarea
			id={id}
			name={name}
			value={value}
			onChange={e => setValue(e.target.value)}
			className={className}
		/>
	</div>

TextArea.propTypes = {
	value: PropTypes.string,
	setValue: PropTypes.func,
	id: PropTypes.string,
	name: PropTypes.string,
	className: PropTypes.string,
}