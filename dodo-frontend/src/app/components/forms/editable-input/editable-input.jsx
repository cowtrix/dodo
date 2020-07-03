import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { Icon } from '../../icon'

import styles from './editable-input.module.scss'

export const EditableInput = ({ name, id, type, value, setValue, error }) => {
	const [editMode, setEditMode] = useState(false)
	return (
		<div className={styles.inputWrapper}>
			<label htmlFor={id} className={error ? styles.error : ''}><h3>{name}</h3></label>
			<div className={styles.input}>
				{!editMode ?
					<span>{value}</span> :
					<input
						type={type}
						id={id}
						name={name}
						value={value}
						onChange={e => setValue(e.target.value)}
						className={error ? styles.error : ''}
					/>
				}
			<Icon icon="edit" onClick={() => setEditMode(!editMode)} size="1x"/>
			</div>
		</div>
	)
}

EditableInput.propTypes = {
	type: PropTypes.string,
	id: PropTypes.string,
	name: PropTypes.string,
	value: PropTypes.string,
	error: PropTypes.string,
	setValue: PropTypes.func,
}