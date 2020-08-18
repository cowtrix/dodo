import React from 'react'
import PropTypes from 'prop-types'
import styles from './input.module.scss'

export const Input = ({ name, id, type, value, setValue, maxLength, error }) =>
	<div className={styles.inputWrapper}>
		<label htmlFor={id} className={error ? styles.error : ''}>
			<h3>{name}</h3>
		</label>
		<input
			type={type}
			id={id}
			name={name}
			value={value}
			onChange={e => setValue(e.target.value)}
			className={error ? styles.error : ''}
			maxLength={maxLength}
		/>
	</div>

Input.propTypes = {
	type: PropTypes.string,
	id: PropTypes.string,
	name: PropTypes.string,
	value: PropTypes.string,
	error: PropTypes.bool,
	setValue: PropTypes.func,
	maxLength: PropTypes.number
}
