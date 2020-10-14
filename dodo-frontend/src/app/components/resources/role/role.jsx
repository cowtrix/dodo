import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { SignUpButton } from '../sign-up-button'
import { TextArea } from '../../forms'
import styles from './role.module.scss'

const APPLICANT_ANSWER_INPUT = 'APPLICANT_ANSWER_INPUT'
const APPLY_NOW = 'Apply Now'
const VIEW_APPLICATION = 'View Application'
const ALREADY_APPLIED = 'You have already applied for this role'
const DEFAULT_GUID = '00000000-0000-0000-0000-000000000000'

export const Role = ({ applicantQuestion, isLoggedIn, applyForRole, resourceColor, hasApplied }) => {
	const [applicantAnswer, setApplicantAnswer] = useState('')
	return (
		isLoggedIn && applicantQuestion && hasApplied == DEFAULT_GUID ?
			<div className={styles.role}>
				<div>
					{applicantQuestion}
				</div>
				<TextArea
					type="text"
					id={APPLICANT_ANSWER_INPUT}
					value={applicantAnswer}
					setValue={setApplicantAnswer}
					className={styles.textArea}
				/>
				<SignUpButton
					onClick={() => applyForRole(applicantAnswer)}
					CTA={APPLY_NOW}
					resourceColor={resourceColor}
					className={styles.signUpButton}
				/>
			</div> :
			hasApplied && hasApplied != DEFAULT_GUID ?
				<div>
					<h2>{ALREADY_APPLIED}</h2>
					<h2><a href={'/roleapplication/' + hasApplied}>{VIEW_APPLICATION}</a></h2>
				</div> :
				null
	)
}


Role.propTypes = {
	applicantQuestion: PropTypes.string,
	hasApplied: PropTypes.bool,
	isLoggedIn: PropTypes.bool,
	applyForRole: PropTypes.func,
	resourceColor: PropTypes.string,
}
