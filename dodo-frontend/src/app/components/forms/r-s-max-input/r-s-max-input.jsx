import React from 'react'
import { components } from 'react-select'

export const RSMaxInput = (props) =>
	<components.Input {...props} maxLength={128} />
